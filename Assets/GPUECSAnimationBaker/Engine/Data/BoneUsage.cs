using System;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [Serializable]
    public struct BoneUsage
    {
        [Tooltip("The default max number of bones to take into account for baking (unless specified per LoD underneath)")]
        public int numberOfBonesPerVertex;
        public BoneUsagePerLoD[] boneUsagesPerLoD;
    }
}