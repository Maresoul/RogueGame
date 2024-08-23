using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace GPUECSAnimationBaker.Samples.SampleScenes._0_Basics
{
    public class StartStopGpuEcsAnimatorScript : MonoBehaviour
    {
        public void StartAnimation()
        {
            StartStopScriptSystem startStopSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<StartStopScriptSystem>();
            startStopSystem.doStart = true;
        }

        public void StopAnimation()
        {
            StartStopScriptSystem startStopSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<StartStopScriptSystem>();
            startStopSystem.doStop = true;
        }
    }
    
    public partial class StartStopScriptSystem : SystemBase
    {
        public bool doStart;
        public bool doStop;
        
        protected override void OnUpdate()
        {
            if (doStart)
            {
                doStart = false;
                Entities.ForEach((GpuEcsAnimatorAspect gpuEcsAnimatorAspect) =>
                {
                    gpuEcsAnimatorAspect.StartAnimation();
                }).WithoutBurst().Run();
            }
            
            if (doStop)
            {
                doStop = false;
                Entities.ForEach((GpuEcsAnimatorAspect gpuEcsAnimatorAspect) =>
                {
                    gpuEcsAnimatorAspect.StopAnimation();
                }).WithoutBurst().Run();
            }
        }
    }
    
}
