using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace GPUECSAnimationBaker.Samples.SampleScenes._2_Transitions
{
    public class TransitionScript : MonoBehaviour
    {
        public Scrollbar transitionSpeedScrollbar;
        public AnimationIdsCh19 animationId;

        public void Transition()
        {
            TransitionScriptSystem blendScriptSystem =
                World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TransitionScriptSystem>();
            blendScriptSystem.doTransition = true;
            blendScriptSystem.animationId = (int)animationId; 
            blendScriptSystem.transitionSpeed = transitionSpeedScrollbar.value;
        }
    }
    
    public partial class TransitionScriptSystem : SystemBase
    {
        public bool doTransition;
        public float transitionSpeed;
        public int animationId;
        
        protected override void OnUpdate()
        {
            if (doTransition)
            {
                doTransition = false;
                Entities.ForEach((GpuEcsAnimatorAspect gpuEcsAnimatorAspect) =>
                {
                    gpuEcsAnimatorAspect.RunAnimation(animationId, transitionSpeed: transitionSpeed);
                }).WithoutBurst().Run();
            }
        }
    }
    
}
