using System;
using Unity.Entities;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [Serializable]
    public struct GpuEcsAnimationBakerData
    {
        public AnimationData[] animations;
        [Tooltip("Specifies if an animation Ids enum code file needs to be generated")]
        public bool generateAnimationIdsEnum;
        [Tooltip("Specifies the name of the animation Ids enum to be generated")]
        public string animationIdsEnumName;
        
        [Tooltip("Specifies if a predefined event IDs list should be used when searching for events")]
        public bool usePredefinedAnimationEventIds;
        [Tooltip("Predefined event IDs list used when searching for events")]
        public string[] predefinedAnimationEventIds;
        [Tooltip("Specifies if an animation event Ids enum code file needs to be generated")]
        public bool generateAnimationEventIdsEnum;
        [Tooltip("Specifies the name of the animation event Ids enum to be generated")]
        public string animationEventIdsEnumName;
        
        public AttachmentAnchor[] attachmentAnchors;
        [Tooltip("Specifies if an attachment anchor Ids enum code file needs to be generated")]
        public bool generateAttachmentAnchorIdsEnum;
        [Tooltip("Specifies the name of the attachment anchor Ids enum to be generated")]
        public string attachmentAnchorIdsEnumName;
        
        public BoneUsage boneUsage;
        [Tooltip("Specifies the TransformUsageFlags to be used when converting the parent animator to an ECS entity")]
        
        public TransformUsageFlags transformUsageFlagsParent;
        [Tooltip("Specifies the TransformUsageFlags to be used when converting the child meshes to an ECS entity")]
        public TransformUsageFlags transformUsageFlagsChildren;
    }
}