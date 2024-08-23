using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Festival.CrowdSpawnerSystem
{
    public class CrowdSpawnerBehaviour : MonoBehaviour
    {
        public float spacing;
        public GameObject[] gpuEcsAnimatorPrefabs; 
    }

    public class CrowdSpawnerBaker : Baker<CrowdSpawnerBehaviour>
    {
        public override void Bake(CrowdSpawnerBehaviour authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new CrowdSpawnerComponent()
            {
                cols = 0,
                rows = 0,
                spacing = authoring.spacing
            });

            AddComponent(entity, new CrowdSpawnerUpdateComponent()
            {
                cols = 0,
                rows = 0,
                updateTime = 0,
                random = Random.CreateFromIndex((uint)Mathf.RoundToInt(Time.time))
            });

            DynamicBuffer<CrowdSpawnerAnimatorPrefabBufferElement> crowdSpawnerAnimatorPrefabs 
                = AddBuffer<CrowdSpawnerAnimatorPrefabBufferElement>(entity);
            foreach(GameObject gpuEcsAnimatorPrefab in authoring.gpuEcsAnimatorPrefabs)
            {
                crowdSpawnerAnimatorPrefabs.Add(new CrowdSpawnerAnimatorPrefabBufferElement()
                {
                    gpuEcsAnimatorPrefab = GetEntity(gpuEcsAnimatorPrefab, 
                        gpuEcsAnimatorPrefab.GetComponent<GpuEcsAnimatorBehaviour>().transformUsageFlags)
                });
            }
            
            AddBuffer<CrowdSpawnerAnimatorBufferElement>(entity);
        }
    }
}