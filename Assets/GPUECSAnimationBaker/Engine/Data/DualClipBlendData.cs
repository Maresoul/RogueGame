using System;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [Serializable]
    public struct DualClipBlendData
    {
        [Tooltip("The animator parameter that will be used to blend")]
        public string blendParameterName;
        public DualClipBlendAnimationClip clip1;
        public DualClipBlendAnimationClip clip2;
        
        [Tooltip("The nbr of samples to take. More will result in better results but larger baked animation textures")]
        public int nbrOfInBetweenSamples;
    }
                    
    [Serializable]
    public struct DualClipBlendAnimationClip
    {
        [Tooltip("The animator parameter value to blend from or to")]
        public float parameterValue;
        [Tooltip("Only used to determine the length of the animation.")]
        public AnimationClip animationClip;
    }
}