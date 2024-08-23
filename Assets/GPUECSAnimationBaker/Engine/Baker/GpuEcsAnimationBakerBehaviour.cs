using System;
using GpuEcsAnimationBaker.Engine.Data;
using Unity.Entities;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.Baker
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class GpuEcsAnimationBakerBehaviour : MonoBehaviour
    {
        public GpuEcsAnimationBakerData bakerData = new GpuEcsAnimationBakerData()
        {
            animations = Array.Empty<AnimationData>(),
            generateAnimationIdsEnum = false,
            animationIdsEnumName = "",
            attachmentAnchors = Array.Empty<AttachmentAnchor>(),
            generateAttachmentAnchorIdsEnum = false,
            attachmentAnchorIdsEnumName = "",
            boneUsage = new BoneUsage()
            {
                numberOfBonesPerVertex = 6,
                boneUsagesPerLoD = Array.Empty<BoneUsagePerLoD>()
            },
            transformUsageFlagsParent = TransformUsageFlags.Dynamic,
            transformUsageFlagsChildren = TransformUsageFlags.Renderable
        };
        [Tooltip("The last generated GPU ECS animator. This is used so that existing references will not be lost after regeneration")]
        public GameObject gpuEcsAnimator;
    }
}