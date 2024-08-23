using System;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    [RequireComponent(typeof(GpuEcsAttachmentBehaviour))]
    public class GpuEcsAttachmentInitializerBehaviour : MonoBehaviour
    {
        public virtual int GetAttachmentAnchorID() { return 0; }
    }

    [RequireComponent(typeof(GpuEcsAttachmentBehaviour))]
    public class GpuEcsAttachmentInitializerBehaviour<T> : GpuEcsAttachmentInitializerBehaviour where T : Enum
    {
        public T attachmentAnchorId;

        public override int GetAttachmentAnchorID()
        {
            Array values = Enum.GetValues(typeof(T));
            for (int i = 0; i < values.Length; i++)
                if((values.GetValue(i)).Equals(attachmentAnchorId)) return i;
            return 0;
        }
    }
}