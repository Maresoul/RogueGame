using Unity.Entities;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Marathon.RunnerSystems
{
    public struct RunnerStateComponent : IComponentData
    {
        public float speed;
        public float fieldSizeZ;
    }
}