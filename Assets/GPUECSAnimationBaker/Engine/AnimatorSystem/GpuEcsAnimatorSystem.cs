using GpuEcsAnimationBaker.Engine.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    [BurstCompile]
    public partial struct GpuEcsAnimatorSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EndSimulationEntityCommandBufferSystem.Singleton ecbSystem =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer.ParallelWriter ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            float deltaTime = SystemAPI.Time.DeltaTime;
            state.Dependency = new GpuEcsAnimatorJob()
            {
                ecb = ecb,
                deltaTime = deltaTime
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct GpuEcsAnimatorJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float deltaTime;

            public void Execute(
                ref GpuEcsAnimatorShaderDataComponent gpuEcsAnimatorShaderData,
                ref GpuEcsAnimatorTransitionInfoComponent gpuEcsAnimatorTransitionInfo,
                ref GpuEcsAnimatorStateComponent gpuEcsAnimatorState,
                ref GpuEcsAnimatorInitializedComponent gpuEcsAnimatorInitialized,
                ref DynamicBuffer<GpuEcsCurrentAttachmentAnchorBufferElement> gpuEcsCurrentAttachmentAnchors,
                ref GpuEcsAnimatorControlStateComponent gpuEcsAnimatorControlState,
                ref DynamicBuffer<GpuEcsAnimatorEventBufferElement> gpuEcsAnimatorEventBuffer,
                in GpuEcsAnimatorControlComponent gpuEcsAnimatorControl,
                in GpuEcsAnimationDataComponent gpuEcsAnimationData,
                in DynamicBuffer<GpuEcsAnimationDataBufferElement> gpuEcsAnimationDataBuffer,
                in DynamicBuffer<GpuEcsAnimationEventOccurenceBufferElement> gpuEcsAnimationEventOccurenceBuffer,
                in DynamicBuffer<GpuEcsAttachmentAnchorDataBufferElement> gpuEcsAttachmentAnchorData,
                [ChunkIndexInQuery] int sortKey, Entity gpuEcsAnimatorEntity)
            {
                gpuEcsAnimatorEventBuffer.Clear();
                if (!gpuEcsAnimatorInitialized.initialized )
                {
                    // We switch immediately to the first animation, no transition
                    gpuEcsAnimatorTransitionInfo = new GpuEcsAnimatorTransitionInfoComponent()
                    {
                        current = gpuEcsAnimatorControl.animatorInfo,
                        blendPreviousToCurrent = 1f
                    };
                    gpuEcsAnimatorState = new GpuEcsAnimatorStateComponent()
                    {
                        currentNormalizedTime = gpuEcsAnimatorControl.startNormalizedTime,
                        stoppedPrevious = false,
                        stoppedCurrent = false
                    };
                    gpuEcsAnimatorInitialized.initialized = true;
                }
                else if(gpuEcsAnimatorControl.animatorInfo.animationID != gpuEcsAnimatorTransitionInfo.current.animationID|| gpuEcsAnimatorControlState.reset)
                {
                    gpuEcsAnimatorControlState.reset = false;
                    // A new animation (or animation combination) has been started, so we need to do a transition
                    // from the old one to the new
                    gpuEcsAnimatorTransitionInfo = new GpuEcsAnimatorTransitionInfoComponent()
                    {
                        current = gpuEcsAnimatorControl.animatorInfo,
                        previous = gpuEcsAnimatorTransitionInfo.current,
                        blendPreviousToCurrent = 0f
                    };
                    gpuEcsAnimatorState = new GpuEcsAnimatorStateComponent()
                    {
                        previousNormalizedTime = gpuEcsAnimatorState.currentNormalizedTime,
                        stoppedPrevious = gpuEcsAnimatorState.stoppedCurrent,
                        currentNormalizedTime = gpuEcsAnimatorControl.startNormalizedTime,
                        stoppedCurrent = false,
                    };
                }
                else
                {
                    // The same animation (or animation combination) is still running, but the parameters might have changed
                    // (blendPrimaryToSecondary or speedFactor)
                    gpuEcsAnimatorTransitionInfo.current = gpuEcsAnimatorControl.animatorInfo;
                }

                GpuEcsAnimatorControlStates controlState = gpuEcsAnimatorControlState.state;
                if (gpuEcsAnimatorControlState.state == GpuEcsAnimatorControlStates.Start)
                    gpuEcsAnimatorState.stoppedCurrent = false;
                else if (gpuEcsAnimatorControlState.state == GpuEcsAnimatorControlStates.Stop)
                    gpuEcsAnimatorState.stoppedCurrent = true;
                gpuEcsAnimatorControlState.state = GpuEcsAnimatorControlStates.KeepCurrentState;

                if (!gpuEcsAnimatorState.stoppedCurrent)
                {
                    UpdateAnimatorState(ref gpuEcsAnimatorState.currentNormalizedTime, ref gpuEcsAnimatorState.stoppedCurrent,
                        ref gpuEcsAnimatorEventBuffer,
                        gpuEcsAnimatorTransitionInfo.current, controlState, gpuEcsAnimationDataBuffer, gpuEcsAnimationEventOccurenceBuffer,
                        out float primaryBlendFactor, out float primaryTransitionToNextFrame, out int primaryFrameIndex,
                        out float secondaryBlendFactor, out float secondaryTransitionToNextFrame, out int secondaryFrameIndex,
                        sortKey, gpuEcsAnimatorEntity, forPrevious: false);
                    if (gpuEcsAnimatorTransitionInfo.blendPreviousToCurrent >= 1f)
                    {
                        gpuEcsAnimatorShaderData.shaderData = new float4x4(
                            primaryBlendFactor, primaryTransitionToNextFrame, primaryFrameIndex, 0,
                            secondaryBlendFactor, secondaryTransitionToNextFrame, secondaryFrameIndex, 0,
                            0, 0, 0, 0,
                            0, 0, 0, 0);

                        //Apply attachment anchor transforms
                        for (int attachmentAnchorIndex = 0; attachmentAnchorIndex < gpuEcsAnimationData.nbrOfAttachmentAnchors; attachmentAnchorIndex++)
                        {
                            int baseIndex = gpuEcsAnimationData.totalNbrOfFrames * attachmentAnchorIndex;
                            gpuEcsCurrentAttachmentAnchors[attachmentAnchorIndex] = new GpuEcsCurrentAttachmentAnchorBufferElement()
                            {
                                currentTransform = LerpBlend(gpuEcsAttachmentAnchorData, baseIndex,
                                    primaryFrameIndex, secondaryFrameIndex,
                                    primaryTransitionToNextFrame, secondaryTransitionToNextFrame,
                                    secondaryBlendFactor)
                            };
                        }
                    }
                    else
                    {
                        if (gpuEcsAnimatorControl.transitionSpeed == 0) gpuEcsAnimatorTransitionInfo.blendPreviousToCurrent = 1f;
                        else
                        {
                            gpuEcsAnimatorTransitionInfo.blendPreviousToCurrent += deltaTime / gpuEcsAnimatorControl.transitionSpeed;
                            if (gpuEcsAnimatorTransitionInfo.blendPreviousToCurrent > 1f) gpuEcsAnimatorTransitionInfo.blendPreviousToCurrent = 1f;
                        }

                        float previousToCurrent = gpuEcsAnimatorTransitionInfo.blendPreviousToCurrent;
                        float currentToPrevious = 1f - previousToCurrent;
                        UpdateAnimatorState(ref gpuEcsAnimatorState.previousNormalizedTime, ref gpuEcsAnimatorState.stoppedPrevious,
                            ref gpuEcsAnimatorEventBuffer,
                            gpuEcsAnimatorTransitionInfo.previous, controlState, gpuEcsAnimationDataBuffer, gpuEcsAnimationEventOccurenceBuffer,
                            out float previousPrimaryBlendFactor, out float previousPrimaryTransitionToNextFrame, out int previousPrimaryFrameIndex,
                            out float previousSecondaryBlendFactor, out float previousSecondaryTransitionToNextFrame, out int previousSecondaryFrameIndex,
                            sortKey, gpuEcsAnimatorEntity, forPrevious: true);

                        gpuEcsAnimatorShaderData.shaderData = new float4x4(
                            previousToCurrent * primaryBlendFactor, primaryTransitionToNextFrame, primaryFrameIndex, 0,
                            previousToCurrent * secondaryBlendFactor, secondaryTransitionToNextFrame, secondaryFrameIndex, 0,
                            currentToPrevious * previousPrimaryBlendFactor, previousPrimaryTransitionToNextFrame, previousPrimaryFrameIndex, 0,
                            currentToPrevious * previousSecondaryBlendFactor, previousSecondaryTransitionToNextFrame, previousSecondaryFrameIndex, 0);

                        for (int attachmentAnchorIndex = 0; attachmentAnchorIndex < gpuEcsAnimationData.nbrOfAttachmentAnchors; attachmentAnchorIndex++)
                        {
                            int baseIndex = gpuEcsAnimationData.totalNbrOfFrames * attachmentAnchorIndex;

                            float4x4 current = LerpBlend(gpuEcsAttachmentAnchorData, baseIndex,
                                primaryFrameIndex, secondaryFrameIndex,
                                primaryTransitionToNextFrame, secondaryTransitionToNextFrame,
                                secondaryBlendFactor);
                            float4x4 previous = LerpBlend(gpuEcsAttachmentAnchorData, baseIndex,
                                previousPrimaryFrameIndex, previousSecondaryFrameIndex,
                                previousPrimaryTransitionToNextFrame, previousSecondaryTransitionToNextFrame,
                                previousSecondaryBlendFactor);

                            gpuEcsCurrentAttachmentAnchors[attachmentAnchorIndex] = new GpuEcsCurrentAttachmentAnchorBufferElement()
                            {
                                currentTransform = LerpTransform(previous, current, previousToCurrent)
                            };
                        }
                    }
                }
            }
            
            private float4x4 LerpBlend(in DynamicBuffer<GpuEcsAttachmentAnchorDataBufferElement> gpuEcsAttachmentAnchorData,
                int baseIndex, int frameIndexA, int frameIndexB, 
                float frameIndexATransitionToNextFrame, float frameIndexBTransitionToNextFrame,
                float t)
            {
                float4x4 result;
                if (t == 0)
                    result = LerpNextFrame(gpuEcsAttachmentAnchorData, baseIndex, frameIndexA, frameIndexATransitionToNextFrame);
                else if(t == 1f)
                    result = LerpNextFrame(gpuEcsAttachmentAnchorData, baseIndex, frameIndexB, frameIndexBTransitionToNextFrame);
                else
                {
                    float4x4 primary = LerpNextFrame(gpuEcsAttachmentAnchorData, baseIndex, frameIndexA, frameIndexATransitionToNextFrame);
                    float4x4 secondary = LerpNextFrame(gpuEcsAttachmentAnchorData, baseIndex, frameIndexB, frameIndexBTransitionToNextFrame);
                    result = LerpTransform(primary, secondary, t); 
                }
                return result;
            }

            private float4x4 LerpNextFrame(in DynamicBuffer<GpuEcsAttachmentAnchorDataBufferElement> gpuEcsAttachmentAnchorData,
                int baseIndex, int frameIndex, float transitionToNextFrame)
            {
                return LerpTransform(
                    gpuEcsAttachmentAnchorData[baseIndex + frameIndex].anchorTransform,
                    gpuEcsAttachmentAnchorData[baseIndex + frameIndex + 1].anchorTransform,
                    transitionToNextFrame
                );
            }

            private float4x4 LerpTransform(float4x4 valueA, float4x4 valueB, float t)
            {
                float3 posA = new float3(valueA.c3.x, valueA.c3.y, valueA.c3.z);
                quaternion rotA = new quaternion(valueA);
                float3 posB = new float3(valueB.c3.x, valueB.c3.y, valueB.c3.z);
                quaternion rotB = new quaternion(valueB);

                float3 pos = math.lerp(posA, posB, t);
                Quaternion rot = math.slerp(rotA, rotB, t);
                return float4x4.TRS(pos, rot, new float3(1f, 1f, 1f));
            }

            private void UpdateAnimatorState(ref float normalizedTime, ref bool stopped, 
                ref DynamicBuffer<GpuEcsAnimatorEventBufferElement> gpuEcsAnimatorEventBuffer,
                AnimatorInfo animatorInfo,
                in GpuEcsAnimatorControlStates controlState,
                in DynamicBuffer<GpuEcsAnimationDataBufferElement> gpuEcsAnimationDataBuffer,
                in DynamicBuffer<GpuEcsAnimationEventOccurenceBufferElement> gpuEcsAnimationEventOccurenceBuffer,
                out float primaryBlendFactor, out float primaryTransitionToNextFrame, out int primaryFrameIndex,
                out float secondaryBlendFactor, out float secondaryTransitionToNextFrame, out int secondaryFrameIndex,
                in int sortKey, Entity gpuEcsAnimatorEntity, bool forPrevious)
            {
                GpuEcsAnimationDataBufferElement animationData = gpuEcsAnimationDataBuffer[animatorInfo.animationID];

                if (animationData.nbrOfInBetweenSamples == 1)
                {
                    float blendSpeedAdjustment = 1f;
                    UpdateAnimationNormalizedTime(ref normalizedTime, ref stopped, ref gpuEcsAnimatorEventBuffer, animatorInfo, controlState, 
                        gpuEcsAnimationEventOccurenceBuffer, animationData, blendSpeedAdjustment, 
                        out float transitionToNextFrame, out int relativeFrameIndex, sortKey, gpuEcsAnimatorEntity, forPrevious);
                    primaryBlendFactor = 1;
                    primaryTransitionToNextFrame = transitionToNextFrame;
                    primaryFrameIndex = animationData.startFrameIndex + relativeFrameIndex;
                    secondaryBlendFactor = 0;
                    secondaryTransitionToNextFrame = 0;
                    secondaryFrameIndex = 0;
                }
                else
                {
                    float endBlend = (float)(animationData.nbrOfInBetweenSamples - 1);
                    float currentBlendSetFloat = animatorInfo.blendFactor * endBlend;
                    int currentBlendSet = (int)math.floor(currentBlendSetFloat);
                    float transitionToNextSet = currentBlendSetFloat - (float)currentBlendSet;
                    
                    float blendSpeedAdjustment = animatorInfo.blendFactor * animationData.blendTimeCorrection + (1f - animatorInfo.blendFactor);
                    UpdateAnimationNormalizedTime(ref normalizedTime, ref stopped, ref gpuEcsAnimatorEventBuffer, animatorInfo, controlState, 
                        gpuEcsAnimationEventOccurenceBuffer, animationData, blendSpeedAdjustment, 
                        out float transitionToNextFrame, out int relativeFrameIndex, sortKey, gpuEcsAnimatorEntity, forPrevious);
                    primaryBlendFactor = 1f - transitionToNextSet;
                    primaryTransitionToNextFrame = transitionToNextFrame;
                    primaryFrameIndex = animationData.startFrameIndex + currentBlendSet * animationData.nbrOfFramesPerSample + relativeFrameIndex;
                    secondaryBlendFactor = transitionToNextSet;
                    secondaryTransitionToNextFrame = transitionToNextFrame;
                    secondaryFrameIndex = animationData.startFrameIndex + (currentBlendSet + 1) * animationData.nbrOfFramesPerSample + relativeFrameIndex;
                }
            }

            private void UpdateAnimationNormalizedTime(ref float normalizedTime, ref bool stopped,
                ref DynamicBuffer<GpuEcsAnimatorEventBufferElement> gpuEcsAnimatorEventBuffer,
                AnimatorInfo animatorInfo, in GpuEcsAnimatorControlStates controlState,
                in DynamicBuffer<GpuEcsAnimationEventOccurenceBufferElement> gpuEcsAnimationEventOccurenceBuffer,
                GpuEcsAnimationDataBufferElement animationData, float blendSpeedAdjustment, 
                out float transitionToNextFrame, out int relativeFrameIndex, int sortKey, Entity gpuEcsAnimatorEntity, bool forPrevious)
            {
                int endFrame = animationData.nbrOfFramesPerSample - 1;
                float animationLength = (float)endFrame / GlobalConstants.SampleFrameRate;
                float currentTime = normalizedTime * animationLength;
                if(!stopped) currentTime += deltaTime * animatorInfo.speedFactor * blendSpeedAdjustment;
                float normalizedTimeLastUpdate = normalizedTime;
                normalizedTime = currentTime / animationLength;
                
                for (int eventOccurencId = animationData.startEventOccurenceId; eventOccurencId < animationData.startEventOccurenceId + animationData.nbrOfEventOccurenceIds; eventOccurencId++)
                {
                    GpuEcsAnimationEventOccurenceBufferElement occurence = gpuEcsAnimationEventOccurenceBuffer[eventOccurencId];
                    if (normalizedTimeLastUpdate < occurence.eventNormalizedTime && normalizedTime > occurence.eventNormalizedTime)
                    {
                        //Trigger event
                        gpuEcsAnimatorEventBuffer.Add(new GpuEcsAnimatorEventBufferElement()
                        {
                            animationId = animatorInfo.animationID,
                            eventId = occurence.eventId
                        });
                    }
                }

                if (!forPrevious && (animationData.loop || controlState == GpuEcsAnimatorControlStates.Start))
                {
                    while (normalizedTime >= 1f) normalizedTime -= 1f;
                }
                else
                {
                    if (normalizedTime >= 1f)
                    {
                        normalizedTime = 1f;
                        stopped = true;
                    }
                }

                if (normalizedTime == 1f) 
                {
                    relativeFrameIndex = endFrame - 1;
                    transitionToNextFrame = 1f;
                }
                else
                {
                    float relativeFrameIndexFloat = normalizedTime * (float)endFrame;
                    relativeFrameIndex = (int)math.floor(relativeFrameIndexFloat);
                    transitionToNextFrame = relativeFrameIndexFloat - (float)relativeFrameIndex;
                }
            }
        }
    }
}