using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    public class GpuEcsAnimatorBehaviour : MonoBehaviour
    {
        public int totalNbrOfFrames;
        public int nbrOfAttachmentAnchors;
        public GpuEcsAnimationData[] animations;
        public GpuEcsAnimationEventOccurence[] animationEventOccurences;
        public TransformUsageFlags transformUsageFlags = TransformUsageFlags.Dynamic;
        public GpuEcsAttachmentAnchorData attachmentAnchorData;
    }

    [Serializable]
    public struct GpuEcsAnimationEventOccurence
    {
        public float eventNormalizedTime;
        public int eventId;
    }

    public class GpuEcsAnimatorBaker : Baker<GpuEcsAnimatorBehaviour>
    {
        public override void Bake(GpuEcsAnimatorBehaviour authoring)
        {
            Entity entity = GetEntity(authoring.transformUsageFlags);

            AddComponent(entity, new GpuEcsAnimationDataComponent()
            {
                nbrOfAttachmentAnchors = authoring.nbrOfAttachmentAnchors,
                totalNbrOfFrames = authoring.totalNbrOfFrames
            });
            
            DynamicBuffer<GpuEcsAnimationDataBufferElement> gpuEcsAnimationDataBuffer = AddBuffer<GpuEcsAnimationDataBufferElement>(entity);
            for (int animationIndex = 0; animationIndex < authoring.animations.Length; animationIndex++)
            {
                GpuEcsAnimationData gpuEcsAnimationData = authoring.animations[animationIndex];
                gpuEcsAnimationDataBuffer.Add(new GpuEcsAnimationDataBufferElement()
                {
                    startFrameIndex = gpuEcsAnimationData.startFrameIndex,
                    nbrOfFramesPerSample = gpuEcsAnimationData.nbrOfFramesPerSample,
                    nbrOfInBetweenSamples = gpuEcsAnimationData.nbrOfInBetweenSamples,
                    blendTimeCorrection = gpuEcsAnimationData.blendTimeCorrection,
                    startEventOccurenceId = gpuEcsAnimationData.startEventOccurenceId,
                    nbrOfEventOccurenceIds = gpuEcsAnimationData.nbrOfEventOccurenceIds,
                    loop = gpuEcsAnimationData.loop
                });
            }
            
            DynamicBuffer<GpuEcsAnimationEventOccurenceBufferElement> gpuEcsAnimationEventOccurenceBuffer = AddBuffer<GpuEcsAnimationEventOccurenceBufferElement>(entity);
            for (int animationEventOccurenceId = 0; animationEventOccurenceId < authoring.animationEventOccurences.Length; animationEventOccurenceId++)
            {
                GpuEcsAnimationEventOccurence occurence = authoring.animationEventOccurences[animationEventOccurenceId];
                gpuEcsAnimationEventOccurenceBuffer.Add(new GpuEcsAnimationEventOccurenceBufferElement()
                {
                    eventNormalizedTime = occurence.eventNormalizedTime,
                    eventId = occurence.eventId
                });
            }
            
            AddComponent(entity, new GpuEcsAnimatorShaderDataComponent()
            {
                shaderData = new float4x4(
                    1f, 0, 0, 0, 
                    0, 0, 0, 0, 
                    0, 0, 0, 0,
                    0, 0, 0, 0)
            });

            int initialAnimationID = 0;
            GpuEcsAnimatorInitializerBehaviour initializer = authoring.GetComponent<GpuEcsAnimatorInitializerBehaviour>();
            if (initializer != null) initialAnimationID = initializer.GetInitialAnimationID();

            AddComponent(entity, new GpuEcsAnimatorInitializedComponent()
            {
                initialized = false
            });
            
            AddComponent(entity, new GpuEcsAnimatorControlComponent()
            {
                animatorInfo = new AnimatorInfo()
                {
                    animationID = initialAnimationID,
                    blendFactor = 0,
                    speedFactor = 1
                },
                transitionSpeed = 0,
                startNormalizedTime = 0
            });
            AddComponent(entity, new GpuEcsAnimatorControlStateComponent()
            {
                state = GpuEcsAnimatorControlStates.Start
            });
                
            AddComponent<GpuEcsAnimatorTransitionInfoComponent>(entity);
            AddComponent<GpuEcsAnimatorStateComponent>(entity);

            DynamicBuffer<GpuEcsAttachmentAnchorDataBufferElement> anchorDataBuffer = AddBuffer<GpuEcsAttachmentAnchorDataBufferElement>(entity);
            DynamicBuffer<GpuEcsCurrentAttachmentAnchorBufferElement> currentAnchorTransformBuffer = AddBuffer<GpuEcsCurrentAttachmentAnchorBufferElement>(entity);
            if (authoring.attachmentAnchorData != null && authoring.nbrOfAttachmentAnchors > 0)
            {
                int anchorDataLength = authoring.attachmentAnchorData.anchorTransforms.Length;
                NativeArray<GpuEcsAttachmentAnchorDataBufferElement> anchors = new NativeArray<GpuEcsAttachmentAnchorDataBufferElement>(anchorDataLength, Allocator.Temp);
                for (int i = 0; i < anchorDataLength; i++) 
                    anchors[i] = new GpuEcsAttachmentAnchorDataBufferElement() { anchorTransform = authoring.attachmentAnchorData.anchorTransforms[i] };
                anchorDataBuffer.AddRange(anchors);
                anchors.Dispose();

                NativeArray<GpuEcsCurrentAttachmentAnchorBufferElement> currentAnchorTransforms = new NativeArray<GpuEcsCurrentAttachmentAnchorBufferElement>(authoring.nbrOfAttachmentAnchors, Allocator.Temp);
                currentAnchorTransformBuffer.AddRange(currentAnchorTransforms);
                currentAnchorTransforms.Dispose();
            }

            AddBuffer<GpuEcsAnimatorEventBufferElement>(entity);
        }
    }
}