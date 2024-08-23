using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    public class GpuEcsAnimatedMeshBehaviour : MonoBehaviour
    {
        public GpuEcsAnimatorBehaviour animator;
        public TransformUsageFlags transformUsageFlags = TransformUsageFlags.Renderable;
    }
    
    public class GpuEcsAnimatedMeshBaker : Baker<GpuEcsAnimatedMeshBehaviour>
    {
        public override void Bake(GpuEcsAnimatedMeshBehaviour authoring)
        {
            Entity entity = GetEntity(authoring.transformUsageFlags);
            AddComponent(entity, new GpuEcsAnimatedMeshComponent()
            {
                animatorEntity = GetEntity(authoring.animator.gameObject,
                    authoring.animator.transformUsageFlags)
            });
            AddComponent(entity, new GpuEcsMaterialAnimationState()
            {
                Value = new float4x4(
                    1f, 0, 0, 0, 
                    0, 0, 0, 0, 
                    0, 0, 0, 0,
                    0, 0, 0, 0)
            });
            AddComponent(entity, new GpuEcsMaterialEnableAnimation()
            {
                Value = 1
            });
        }
    }
}