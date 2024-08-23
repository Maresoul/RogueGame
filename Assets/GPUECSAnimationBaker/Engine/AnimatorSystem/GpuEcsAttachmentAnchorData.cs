using System;
using Unity.Mathematics;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    [Serializable]
    public class GpuEcsAttachmentAnchorData : ScriptableObject
    {
        public float4x4[] anchorTransforms;
    }
}