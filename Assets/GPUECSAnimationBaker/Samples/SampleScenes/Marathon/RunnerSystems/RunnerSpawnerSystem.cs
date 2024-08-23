using System;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Marathon.RunnerSystems
{
    [BurstCompile]
    public partial struct RunnerSpawnerSystem : ISystem
    {
        private ComponentLookup<LocalTransform> localTransformLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            localTransformLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            localTransformLookup.Update(ref state);
            EndSimulationEntityCommandBufferSystem.Singleton ecbSystem =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer.ParallelWriter ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            state.Dependency = new RunnerSpawnerJob()
            {
                ecb = ecb,
                deltaTime = deltaTime,
                localTransformLookup = localTransformLookup
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct RunnerSpawnerJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float deltaTime;
            [ReadOnly] public ComponentLookup<LocalTransform> localTransformLookup;
            
            public void Execute(
                ref RunnerSpawnerUpdateComponent runnerSpawnerUpdate,
                in RunnerSpawnerComponent runnerSpawner, 
                in DynamicBuffer<RunnerSpawnerAnimatorBufferElement> runnerSpawnerAnimators,
                in DynamicBuffer<RunnerSpawnerAnimatorPrefabBufferElement> runnerSpawnerAnimatorPrefabs,
                Entity runnerSpawnerEntity, [ChunkIndexInQuery] int sortKey)
            {
                double TOLERANCE = 0.01f;
                if (runnerSpawner.nbrOfRunners != runnerSpawnerUpdate.nbrOfRunners
                    || Math.Abs(runnerSpawner.fieldSizeX - runnerSpawnerUpdate.fieldSizeX) > TOLERANCE
                    || Math.Abs(runnerSpawner.fieldSizeZ - runnerSpawnerUpdate.fieldSizeZ) > TOLERANCE)
                {
                    runnerSpawnerUpdate.updateTime -= deltaTime;
                    if (runnerSpawnerUpdate.updateTime <= 0)
                    {
                        // First Delete all existing entities
                        foreach (RunnerSpawnerAnimatorBufferElement runnerSpawnerAnimator in runnerSpawnerAnimators)
                        {
                            ecb.DestroyEntity(sortKey, runnerSpawnerAnimator.gpuEcsAnimator);
                        }

                        DynamicBuffer<RunnerSpawnerAnimatorBufferElement> newCrowdSpawnerAnimators
                            = ecb.SetBuffer<RunnerSpawnerAnimatorBufferElement>(sortKey, runnerSpawnerEntity);
                        newCrowdSpawnerAnimators.Clear();

                        for (int i = 0; i < runnerSpawnerUpdate.nbrOfRunners; i++)
                        {
                            newCrowdSpawnerAnimators.Add(new RunnerSpawnerAnimatorBufferElement()
                            {
                                gpuEcsAnimator = CreateNewAnimator(ref runnerSpawnerUpdate, runnerSpawner, sortKey, runnerSpawnerAnimatorPrefabs)
                            });
                        }

                        ecb.SetComponent<RunnerSpawnerComponent>(sortKey, runnerSpawnerEntity, new RunnerSpawnerComponent()
                        {
                            fieldSizeZ = runnerSpawnerUpdate.fieldSizeZ,
                            fieldSizeX = runnerSpawnerUpdate.fieldSizeX,
                            nbrOfRunners = runnerSpawnerUpdate.nbrOfRunners,
                            speedWalking = runnerSpawner.speedWalking,
                            speedRunning = runnerSpawner.speedRunning,
                            speedSprinting = runnerSpawner.speedSprinting,
                            minSpeed = runnerSpawner.minSpeed,
                            maxSpeed = runnerSpawner.maxSpeed
                        });
                    }
                }
            }

            private Entity CreateNewAnimator(ref RunnerSpawnerUpdateComponent runnerSpawnerUpdate, RunnerSpawnerComponent runnerSpawner,
                int sortKey, in DynamicBuffer<RunnerSpawnerAnimatorPrefabBufferElement> runnerSpawnerAnimatorPrefabs)
            {
                // Select a random runner prefab from the available buffer
                Entity gpuEcsAnimatorPrefab = runnerSpawnerAnimatorPrefabs[
                    runnerSpawnerUpdate.random.NextInt(0, runnerSpawnerAnimatorPrefabs.Length)].gpuEcsAnimatorPrefab;
                 
                // Spawn a new runner
                Entity gpuEcsAnimator = ecb.Instantiate(sortKey, gpuEcsAnimatorPrefab);

                // Place it randomly in the running field
                ecb.SetComponent(sortKey, gpuEcsAnimator, new LocalTransform()
                {
                    Position = new float3(
                        runnerSpawnerUpdate.random.NextFloat(-runnerSpawnerUpdate.fieldSizeX / 2f, +runnerSpawnerUpdate.fieldSizeX / 2f),
                        0,
                        runnerSpawnerUpdate.random.NextFloat(-runnerSpawnerUpdate.fieldSizeZ / 2f, +runnerSpawnerUpdate.fieldSizeZ / 2f)),
                    Rotation = quaternion.identity,
                    Scale = localTransformLookup[gpuEcsAnimatorPrefab].Scale
                });

                // Select a random speed for the new runner between minimum & maximum speed
                float speed = runnerSpawnerUpdate.random.NextFloat(runnerSpawner.minSpeed, runnerSpawner.maxSpeed); 
                ecb.SetComponent(sortKey, gpuEcsAnimator, new RunnerStateComponent()
                {
                    speed = speed,
                    fieldSizeZ = runnerSpawnerUpdate.fieldSizeZ
                });
                
                // Calculate blendFactor, speedFactor & select animation
                float blendFactor;
                AnimationIdsRunnerMarathon animationID;
                float speedFactor;
                if (speed < runnerSpawner.speedWalking)
                {
                    animationID = AnimationIdsRunnerMarathon.WalkToRun;
                    blendFactor = 0;
                    speedFactor = speed / runnerSpawner.speedWalking;
                }
                else if (speed < runnerSpawner.speedRunning)
                {
                    animationID = AnimationIdsRunnerMarathon.WalkToRun;
                    blendFactor = (speed - runnerSpawner.speedWalking) 
                                  / (runnerSpawner.speedRunning - runnerSpawner.speedWalking);
                    speedFactor = 1f;
                }
                else if(speed < runnerSpawner.speedSprinting)
                {
                    animationID = AnimationIdsRunnerMarathon.RunToSprint;
                    blendFactor = (speed - runnerSpawner.speedRunning) 
                                  / (runnerSpawner.speedSprinting - runnerSpawner.speedWalking);
                    speedFactor = 1f;
                }
                else
                {
                    animationID = AnimationIdsRunnerMarathon.RunToSprint;
                    blendFactor = 1f;
                    speedFactor = speed / runnerSpawner.speedSprinting;
                }
                
                // Kick off the correct animation with a random time offset so to avoid synchronized animations
                ecb.SetComponent(sortKey, gpuEcsAnimator, new GpuEcsAnimatorControlComponent()
                {
                    animatorInfo = new AnimatorInfo()
                    {
                        animationID = (int) animationID,
                        blendFactor = blendFactor,
                        speedFactor = speedFactor
                    },
                    startNormalizedTime = runnerSpawnerUpdate.random.NextFloat(0f, 1f),
                    transitionSpeed = 0
                });
                return gpuEcsAnimator;
            }
        }
    }
}