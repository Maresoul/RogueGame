using System;

namespace GpuEcsAnimationBaker.Engine.Data
{
    [Serializable]
    public struct AnimatorParameter
    {
        public string parameterName;
        public AnimatorParameterTypes parameterType;
        public bool boolValue;
        public float floatValue;
        public int intValue;
    }

}