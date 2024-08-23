using System;
using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace GPUECSAnimationBaker.Samples.SampleScenes.Marathon.RunnerSystems
{
    public class RunnerSpawnerBehaviour : MonoBehaviour
    {
        public GameObject[] gpuEcsAnimatorPrefabs; 
        public float walkCycleDistance;
        public AnimationClip walkAnimation;
        public float runCycleDistance;
        public AnimationClip runAnimation;
        public float sprintCycleDistance;
        public AnimationClip sprintAnimation;
        public float minSpeed;
        public float maxSpeed;
    }

    public class RunnerSpawnerBaker : Baker<RunnerSpawnerBehaviour>
    {
        public override void Bake(RunnerSpawnerBehaviour authoring)
        {
            float speedWalking = authoring.walkCycleDistance / authoring.walkAnimation.length;
            float speedRunning = authoring.runCycleDistance / authoring.runAnimation.length;
            float speedSprinting = authoring.sprintCycleDistance / authoring.sprintAnimation.length;
            float minSpeed = Math.Min(authoring.minSpeed, speedWalking);
            float maxSpeed = Math.Max(authoring.maxSpeed, speedSprinting);
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new RunnerSpawnerComponent()
            {
                fieldSizeZ = 0,
                fieldSizeX = 0,
                nbrOfRunners = 0,
                speedWalking = speedWalking,
                speedRunning = speedRunning,
                speedSprinting = speedSprinting,
                minSpeed = minSpeed,
                maxSpeed = maxSpeed
            });
            
            AddComponent(entity, new RunnerSpawnerUpdateComponent()
            {
                fieldSizeZ = 0,
                fieldSizeX = 0,
                nbrOfRunners = 0,
                updateTime = 0,
                random = Random.CreateFromIndex((uint)Mathf.RoundToInt(Time.time))
            });
            
            DynamicBuffer<RunnerSpawnerAnimatorPrefabBufferElement> crowdSpawnerAnimatorPrefabs 
                = AddBuffer<RunnerSpawnerAnimatorPrefabBufferElement>(entity);
            foreach(GameObject gpuEcsAnimatorPrefab in authoring.gpuEcsAnimatorPrefabs)
            {
                crowdSpawnerAnimatorPrefabs.Add(new RunnerSpawnerAnimatorPrefabBufferElement()
                {
                    gpuEcsAnimatorPrefab = GetEntity(gpuEcsAnimatorPrefab, 
                        gpuEcsAnimatorPrefab.GetComponent<GpuEcsAnimatorBehaviour>().transformUsageFlags)
                });
            }

            AddBuffer<RunnerSpawnerAnimatorBufferElement>(entity);
        }
    }
}