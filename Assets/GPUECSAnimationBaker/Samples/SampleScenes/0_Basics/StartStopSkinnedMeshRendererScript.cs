using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace GPUECSAnimationBaker.Samples.SampleScenes._0_Basics
{
    public class StartStopSkinnedMeshRendererScript : MonoBehaviour
    {
        public Animator animator;
        
        public void StartAnimation()
        {
            animator.enabled = true;
        }

        public void StopAnimation()
        {
            animator.enabled = false;
        }
    }
}
