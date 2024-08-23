using Unity.Entities;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    public class GpuEcsAttachmentBehaviour : MonoBehaviour
    {
    }
    
    public class GpuEcsAttachmentBaker : Baker<GpuEcsAttachmentBehaviour>
    {
        public override void Bake(GpuEcsAttachmentBehaviour authoring)
        {
            int attachmentAnchorId = 0;
            Entity gpuEcsAnimatorEntity = Entity.Null;
            
            GpuEcsAttachmentInitializerBehaviour initializer = authoring.GetComponent<GpuEcsAttachmentInitializerBehaviour>();
            if (initializer != null) attachmentAnchorId = initializer.GetAttachmentAnchorID();
            if (authoring.transform.parent != null)
            {
                GpuEcsAnimatorBehaviour gpuEcsAnimator = authoring.transform.parent.GetComponent<GpuEcsAnimatorBehaviour>();
                if (gpuEcsAnimator != null)
                {
                    gpuEcsAnimatorEntity = GetEntity(gpuEcsAnimator, TransformUsageFlags.None);
                }
            }

            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GpuEcsAttachmentComponent()
            {
                gpuEcsAnimatorEntity = gpuEcsAnimatorEntity,
                attachmentAnchorId = attachmentAnchorId
            });
            
        }
    }
}