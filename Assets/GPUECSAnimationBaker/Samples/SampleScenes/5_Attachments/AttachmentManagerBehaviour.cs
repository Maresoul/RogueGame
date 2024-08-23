using System;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Entities;
using UnityEngine;

namespace GPUECSAnimationBaker.Samples.SampleScenes._5_Attachments
{
    public class AttachmentManagerBehaviour : MonoBehaviour
    {
        public AttachmentPrefabInfo[] attachmentPrefabs;
    }

    [Serializable]
    public class AttachmentPrefabInfo
    {
        public AnchorIdsMariaAttachments anchor;
        public GameObject attachmentPrefab;
    }

    public class AttachmentManagerBaker : Baker<AttachmentManagerBehaviour>
    {
        public override void Bake(AttachmentManagerBehaviour authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            DynamicBuffer<AttachmentPrefabBufferElement> attachmentPrefabBuffer = AddBuffer<AttachmentPrefabBufferElement>(entity);
            for (int prefabIndex = 0; prefabIndex < authoring.attachmentPrefabs.Length; prefabIndex++)
            {
                AttachmentPrefabInfo attachmentPrefabInfo = authoring.attachmentPrefabs[prefabIndex];
                attachmentPrefabBuffer.Add(new AttachmentPrefabBufferElement()
                {
                    anchor = attachmentPrefabInfo.anchor,
                    attachmentPrefab = GetEntity(attachmentPrefabInfo.attachmentPrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}