using Unity.Entities;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    public struct GpuEcsAttachmentComponent : IComponentData
    {
        public Entity gpuEcsAnimatorEntity;
        public int attachmentAnchorId;
    }
}