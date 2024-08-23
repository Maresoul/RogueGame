using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    public partial struct GpuEcsAnimatedMeshSystem : ISystem
    {
        private ComponentLookup<GpuEcsAnimatorShaderDataComponent> gpuEcsAnimatorShaderDataLookup;
            
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            gpuEcsAnimatorShaderDataLookup = state.GetComponentLookup<GpuEcsAnimatorShaderDataComponent>(isReadOnly: true);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            gpuEcsAnimatorShaderDataLookup.Update(ref state);
            state.Dependency = new GpuEcsAnimatedMeshJob()
            {
                gpuEcsAnimatorShaderDataLookup = gpuEcsAnimatorShaderDataLookup
            }.ScheduleParallel(state.Dependency);
        }
        
        [BurstCompile]
        private partial struct GpuEcsAnimatedMeshJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<GpuEcsAnimatorShaderDataComponent> gpuEcsAnimatorShaderDataLookup;

            public void Execute(ref GpuEcsMaterialAnimationState gpuEcsMaterialAnimationState,
                in GpuEcsAnimatedMeshComponent gpuEcsAnimatedMesh)
            {
                GpuEcsAnimatorShaderDataComponent gpuEcsAnimatorShaderData = gpuEcsAnimatorShaderDataLookup[gpuEcsAnimatedMesh.animatorEntity];
                gpuEcsMaterialAnimationState.Value = gpuEcsAnimatorShaderData.shaderData;
            }
        }        
    }
}