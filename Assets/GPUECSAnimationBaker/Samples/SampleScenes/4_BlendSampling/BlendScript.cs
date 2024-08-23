using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GPUECSAnimationBaker.Samples.SampleScenes._4_BlendSampling
{
    public class BlendScript : MonoBehaviour
    {
        public void Blend(float blendValue)
        {
            BlendScriptSystem blendScriptSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<BlendScriptSystem>();
            blendScriptSystem.changeBlend = true;
            blendScriptSystem.blendValue = blendValue;
        }
    }

    public partial class BlendScriptSystem : SystemBase
    {
        public bool changeBlend;
        public float blendValue;
        
        protected override void OnUpdate()
        {
            if (changeBlend)
            {
                changeBlend = false;
                Entities.ForEach((GpuEcsAnimatorAspect gpuEcsAnimatorAspect) =>
                {
                    gpuEcsAnimatorAspect.RunAnimation((int)AnimationIdsMaria.StrafeWalk, blendFactor: blendValue);
                }).WithoutBurst().Run();
            }
        }
    }
}
