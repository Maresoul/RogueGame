using Unity.Entities;
using Unity.Mathematics;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Festival.CrowdSpawnerSystem
{
    public struct CrowdSpawnerComponent : IComponentData
    {
        public int cols;
        public int rows;
        public float spacing;
    }

    public struct CrowdSpawnerUpdateComponent : IComponentData
    {
        public int cols;
        public int rows;
        public Random random;
        public float updateTime;
    }

    public struct CrowdSpawnerAnimatorPrefabBufferElement : IBufferElementData
    {
        public Entity gpuEcsAnimatorPrefab;
    }

    public struct CrowdSpawnerAnimatorBufferElement : IBufferElementData
    {
        public Entity gpuEcsAnimator;
    }
}