using System;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [Serializable]
    public class BoneUsagePerLoD
    {
        [Tooltip("The LOD index this number applies to")]
        public int lodIndex;
        [Tooltip("The max number of bones to take into account for baking for this LOD")]
        public int maxNumberOfBonesPerVertex;
    }
}