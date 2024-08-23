using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace GPUECSAnimationBaker.Samples.SampleScenes._5_Attachments
{
    public class AttachmentScript : MonoBehaviour
    {
        public int attachmentToSetIndex;
        
        public void Attach()
        {
            AttachmentScriptSystem blendScriptSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<AttachmentScriptSystem>();
            blendScriptSystem.changeAttachments = true;
            blendScriptSystem.attachmentToSetIndex = attachmentToSetIndex;
        }
    }

    public partial class AttachmentScriptSystem : SystemBase
    {
        public bool changeAttachments;
        public int attachmentToSetIndex;
        
        protected override void OnUpdate()
        {
            if (changeAttachments)
            {
                changeAttachments = false;

                EntityManager entityManager = World.EntityManager;
                Entity gpuEcsAnimatorEntity = SystemAPI.GetSingletonEntity<AttachmentPrefabBufferElement>();
                DynamicBuffer<AttachmentPrefabBufferElement> attachments = SystemAPI.GetSingletonBuffer<AttachmentPrefabBufferElement>();
                AttachmentPrefabBufferElement attachmentPrefab = attachments[attachmentToSetIndex];
                
                EntityCommandBuffer ecbDelete = new EntityCommandBuffer(Allocator.Temp);
                Entities.ForEach((
                    in GpuEcsAttachmentComponent gpuEcsAttachment,
                    in Entity entity
                ) =>
                {
                    if(gpuEcsAttachment.attachmentAnchorId == (int)attachmentPrefab.anchor)
                        ecbDelete.DestroyEntity(entity); 
                }).Run();
                ecbDelete.Playback(entityManager);

                Entity newAttachment = entityManager.Instantiate(attachmentPrefab.attachmentPrefab);

                entityManager.AddComponent<Parent>(newAttachment);
                entityManager.SetComponentData(newAttachment, new Parent() 
                {
                    Value = gpuEcsAnimatorEntity
                });
                entityManager.AddComponent<GpuEcsAttachmentComponent>(newAttachment);
                entityManager.SetComponentData<GpuEcsAttachmentComponent>(newAttachment, new GpuEcsAttachmentComponent()
                {
                    attachmentAnchorId = (int) attachmentPrefab.anchor,
                    gpuEcsAnimatorEntity = gpuEcsAnimatorEntity
                });
            }
        }
    }
}
