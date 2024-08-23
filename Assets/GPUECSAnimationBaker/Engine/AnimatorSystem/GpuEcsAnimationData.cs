using System;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    [Serializable]
    public class GpuEcsAnimationData
    {
        public int startFrameIndex;
        public int nbrOfFramesPerSample;
        public int nbrOfInBetweenSamples;
        public float blendTimeCorrection;
        public int startEventOccurenceId;
        public int nbrOfEventOccurenceIds;
        public bool loop;
    }
}