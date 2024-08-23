using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Marathon.RunnerSystems
{
    [BurstCompile]
    public partial struct RunnerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new RunnerJob()
            {
                deltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct RunnerJob : IJobEntity
        {
            [ReadOnly] public float deltaTime;
            
            public void Execute(ref LocalTransform localTransform, in RunnerStateComponent runnerState)
            {
                float z = localTransform.Position.z;
                z += deltaTime * runnerState.speed;
                while (z > runnerState.fieldSizeZ / 2f) z -= runnerState.fieldSizeZ; 
                localTransform.Position = new float3(localTransform.Position.x, 0f, z);
            }
        }
    }
}