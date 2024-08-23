using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace GPUECSAnimationBaker.Samples.SampleScenes._6_Events
{
    public class StartGpuEcsAnimatorScript : MonoBehaviour
    {
        public AnimationIdsMaria animationID;
        
        public void StartAnimation()
        {
            StartStopScriptSystem startStopSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<StartStopScriptSystem>();
            startStopSystem.animationIDToStart = animationID;
        }
    }
    
    public partial class StartStopScriptSystem : SystemBase
    {
        public AnimationIdsMaria? animationIDToStart;
        
        protected override void OnUpdate()
        {
            if (animationIDToStart.HasValue)
            {
                Entities.ForEach((GpuEcsAnimatorAspect gpuEcsAnimatorAspect, in GpuEcsAnimatorControlComponent gpuEcsAnimatorControl) =>
                {
                    if(gpuEcsAnimatorControl.animatorInfo.animationID == (int) animationIDToStart.Value)
                        gpuEcsAnimatorAspect.StartAnimation();
                }).WithoutBurst().Run();
                animationIDToStart = null;
            }
        }
    }
    
}
