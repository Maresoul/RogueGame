using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Festival.CrowdSpawnerSystem
{
    [BurstCompile]
    public partial struct CrowdSpawnerSystem : ISystem
    {
        private BufferLookup<GpuEcsAnimationDataBufferElement> gpuEcsAnimationDataBufferLookup;
        private ComponentLookup<LocalTransform> localTransformLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            gpuEcsAnimationDataBufferLookup = state.GetBufferLookup<GpuEcsAnimationDataBufferElement>(isReadOnly: true);
            localTransformLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            gpuEcsAnimationDataBufferLookup.Update(ref state);
            localTransformLookup.Update(ref state);
            EndSimulationEntityCommandBufferSystem.Singleton ecbSystem =
                SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer.ParallelWriter ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
            float deltaTime = SystemAPI.Time.DeltaTime;
            
            state.Dependency = new CrowdSpawnerJob()
            {
                ecb = ecb,
                deltaTime = deltaTime,
                gpuEcsAnimationDataBufferLookup = gpuEcsAnimationDataBufferLookup,
                localTransformLookup = localTransformLookup
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct CrowdSpawnerJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ecb;
            [ReadOnly] public float deltaTime;
            [ReadOnly] public BufferLookup<GpuEcsAnimationDataBufferElement> gpuEcsAnimationDataBufferLookup;
            [ReadOnly] public ComponentLookup<LocalTransform> localTransformLookup;
            
            public void Execute(
                ref CrowdSpawnerUpdateComponent crowdSpawnerUpdate,
                in CrowdSpawnerComponent crowdSpawner,
                in DynamicBuffer<CrowdSpawnerAnimatorBufferElement> crowdSpawnerAnimators,
                in DynamicBuffer<CrowdSpawnerAnimatorPrefabBufferElement> crowdSpawnerAnimatorPrefabs,
                Entity crowdSpawnerEntity, [ChunkIndexInQuery] int sortKey)
            {
                if (crowdSpawner.rows != crowdSpawnerUpdate.rows || crowdSpawner.cols != crowdSpawnerUpdate.cols)
                {
                    crowdSpawnerUpdate.updateTime -= deltaTime;
                    if (crowdSpawnerUpdate.updateTime <= 0)
                    {
                        // First delete all existing entities
                        foreach (CrowdSpawnerAnimatorBufferElement crowdSpawnerAnimator in crowdSpawnerAnimators)
                            ecb.DestroyEntity(sortKey, crowdSpawnerAnimator.gpuEcsAnimator);

                        DynamicBuffer<CrowdSpawnerAnimatorBufferElement> newCrowdSpawnerAnimators
                            = ecb.SetBuffer<CrowdSpawnerAnimatorBufferElement>(sortKey, crowdSpawnerEntity);
                        newCrowdSpawnerAnimators.Clear();

                        // Calculate the base offset so that the square of entities is centered around the origin
                        float3 baseOffset = new float3(
                            -(crowdSpawnerUpdate.cols - 1) * crowdSpawner.spacing / 2f,
                            0f,
                            -(crowdSpawnerUpdate.rows - 1) * crowdSpawner.spacing / 2f);

                        for (int col = 0; col < crowdSpawnerUpdate.cols; col++)
                        {
                            for (int row = 0; row < crowdSpawnerUpdate.rows; row++)
                            {
                                newCrowdSpawnerAnimators.Add(new CrowdSpawnerAnimatorBufferElement()
                                {
                                    gpuEcsAnimator = CreateNewAnimator(ref crowdSpawnerUpdate, crowdSpawner, sortKey,
                                        baseOffset, col, row, crowdSpawnerAnimatorPrefabs)
                                });
                            }
                        }

                        ecb.SetComponent<CrowdSpawnerComponent>(sortKey, crowdSpawnerEntity, new CrowdSpawnerComponent()
                        {
                            cols = crowdSpawnerUpdate.cols,
                            rows = crowdSpawnerUpdate.rows,
                            spacing = crowdSpawner.spacing
                        });
                    }
                }
            }

            private Entity CreateNewAnimator(ref CrowdSpawnerUpdateComponent crowdSpawnerUpdate, CrowdSpawnerComponent crowdSpawner,
                int sortKey, float3 baseOffset, int col, int row,
                in DynamicBuffer<CrowdSpawnerAnimatorPrefabBufferElement> crowdSpawnerAnimatorPrefabs)
            {
                // Select a random prefab from the available buffer
                Entity gpuEcsAnimatorPrefab = crowdSpawnerAnimatorPrefabs[
                    crowdSpawnerUpdate.random.NextInt(0, crowdSpawnerAnimatorPrefabs.Length)].gpuEcsAnimatorPrefab;
                // Spawn a character
                Entity gpuEcsAnimator = ecb.Instantiate(sortKey, gpuEcsAnimatorPrefab);
                
                // set the position according to column, row & spacing values
                // Preserve the scale that was set in the prefab
                ecb.SetComponent(sortKey, gpuEcsAnimator, new LocalTransform()
                {
                    Position = baseOffset + new float3(
                        col * crowdSpawner.spacing + crowdSpawnerUpdate.random.NextFloat(-crowdSpawner.spacing / 4f, crowdSpawner.spacing / 4f)
                        , 0,
                        row * crowdSpawner.spacing + crowdSpawnerUpdate.random.NextFloat(-crowdSpawner.spacing / 4f, crowdSpawner.spacing / 4f)
                    ),
                    Rotation = quaternion.Euler(0, crowdSpawnerUpdate.random.NextFloat(-math.PI, math.PI), 0),
                    Scale = localTransformLookup[gpuEcsAnimatorPrefab].Scale
                });
                
                // Pick a random animation ID from the available animations
                DynamicBuffer<GpuEcsAnimationDataBufferElement> animationDataBuffer = gpuEcsAnimationDataBufferLookup[gpuEcsAnimatorPrefab];
                int animationID = crowdSpawnerUpdate.random.NextInt(0, animationDataBuffer.Length);
                
                // Kick off the correct animation with a random time offset so to avoid synchronized animations
                ecb.SetComponent(sortKey, gpuEcsAnimator, new GpuEcsAnimatorControlComponent()
                {
                    animatorInfo = new AnimatorInfo()
                    {
                        animationID = animationID,
                        blendFactor = 0,
                        speedFactor = 1f
                    },
                    startNormalizedTime = crowdSpawnerUpdate.random.NextFloat(0f, 1f),
                    transitionSpeed = 0
                });
                return gpuEcsAnimator;
            }
        }
    }
}