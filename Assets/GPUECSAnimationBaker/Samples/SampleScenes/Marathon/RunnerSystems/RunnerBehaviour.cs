using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Entities;
using UnityEngine;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Marathon.RunnerSystems
{
    public class RunnerBehaviour : MonoBehaviour
    {
        public float speed;
    }

    public class RunnerBaker : Baker<RunnerBehaviour>
    {
        public override void Bake(RunnerBehaviour authoring)
        {
            Entity entity = GetEntity(authoring.GetComponent<GpuEcsAnimatorBehaviour>().transformUsageFlags);
            AddComponent(entity, new RunnerStateComponent()
            {
                speed = authoring.speed
            });
        }
    }
}