using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    public struct GpuEcsAnimatedMeshComponent : IComponentData
    {
        public Entity animatorEntity;
    }

    [MaterialProperty("_AnimationState")]
    public struct GpuEcsMaterialAnimationState : IComponentData
    {
        public float4x4 Value;
    }

    [MaterialProperty("_EnableAnimation")]
    public struct GpuEcsMaterialEnableAnimation : IComponentData
    {
        public float Value;
    }
    
}