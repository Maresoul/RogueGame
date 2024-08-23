using System;
using UnityEngine;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [Serializable]
    public struct AnimationData
    {
        [Tooltip("Only used when generating enum code file to identify animations")]
        public string animationID;
        [Tooltip("The main animator state name that will be used during sampling, searched across all layers")]
        public string animatorStateName;
        [Tooltip("Specify single clip or dual clip blend animations")]
        public AnimationTypes animationType;
        public SingleClipData singleClipData;
        public DualClipBlendData dualClipBlendData;
        [Tooltip("Specify whether the animation should loop")]
        public bool loop;
        [Tooltip("Any additional Animator parameters that will be used during sampling (independent of blending)\nSpecify name, type & value for each")]
        public AnimatorParameter[] additionalAnimatorParameterValues;
        [Tooltip("Any additional Animator states that need to be set on different layers")]
        public AnimatorState[] additionalAnimatorStatesPerLayer;
    }

}