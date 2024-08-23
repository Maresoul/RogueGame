using System;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [Serializable]
    public struct AttachmentAnchor
    {
        [Tooltip("Only used when generating attachments code file to identify anchors")]
        public string attachmentAnchorID;
        [Tooltip("Reference to the attachment transform inside the bone hierarchy")]
        public Transform attachmentAnchorTransform;
    }
}