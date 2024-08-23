using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    [BurstCompile]
    public partial struct GpuEcsAttachmentSystem : ISystem
    {
        private BufferLookup<GpuEcsCurrentAttachmentAnchorBufferElement> gpuEcsCurrentAttachmentAnchorLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            gpuEcsCurrentAttachmentAnchorLookup = state.GetBufferLookup<GpuEcsCurrentAttachmentAnchorBufferElement>(isReadOnly: true);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            gpuEcsCurrentAttachmentAnchorLookup.Update(ref state);
            state.Dependency = new GpuEcsAttachmentJob()
            {
                gpuEcsCurrentAttachmentAnchorLookup = gpuEcsCurrentAttachmentAnchorLookup
            }.ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct GpuEcsAttachmentJob : IJobEntity
        {
            [ReadOnly] public BufferLookup<GpuEcsCurrentAttachmentAnchorBufferElement> gpuEcsCurrentAttachmentAnchorLookup;
            
            public void Execute(ref LocalTransform localTransform, in GpuEcsAttachmentComponent gpuEcsAttachment)
            {
                DynamicBuffer<GpuEcsCurrentAttachmentAnchorBufferElement> currentAttachmentAnchors 
                    = gpuEcsCurrentAttachmentAnchorLookup[gpuEcsAttachment.gpuEcsAnimatorEntity];
                GpuEcsCurrentAttachmentAnchorBufferElement currentAnchor = currentAttachmentAnchors[gpuEcsAttachment.attachmentAnchorId];
                localTransform = LocalTransform.FromMatrix(currentAnchor.currentTransform);
            }
        }
    }
}