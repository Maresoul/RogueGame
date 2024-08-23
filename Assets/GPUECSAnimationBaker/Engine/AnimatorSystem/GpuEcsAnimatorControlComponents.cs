using Unity.Entities;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    public struct GpuEcsAnimatorControlComponent : IComponentData
    {
        public AnimatorInfo animatorInfo; // All info about the animation you want to play
        public float startNormalizedTime; // An option to start the animation from an arbitrary position (0 to 1)
        public float transitionSpeed; // The transition speed that will be applied when switching to another animation
    }

    public struct AnimatorInfo
    {
        public int animationID; // the unique animation ID, can be assigned from the generated enum file
        public float blendFactor; // From 0 to 1, going from clip1 to clip2
        public float speedFactor; // <1 to make the animation go slower, >1 to make it go faster
    }

    public enum GpuEcsAnimatorControlStates
    {
        Start,
        Stop,
        KeepCurrentState
    }

    public struct GpuEcsAnimatorControlStateComponent : IComponentData
    {
        public GpuEcsAnimatorControlStates state;
        public bool reset;
    }
}