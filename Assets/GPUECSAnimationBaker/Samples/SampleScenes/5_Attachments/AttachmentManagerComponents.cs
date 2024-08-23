using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Entities;

namespace GPUECSAnimationBaker.Samples.SampleScenes._5_Attachments
{
    public struct AttachmentPrefabBufferElement : IBufferElementData
    {
        public AnchorIdsMariaAttachments anchor;
        public Entity attachmentPrefab;
    }
}