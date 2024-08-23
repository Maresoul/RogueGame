#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.Baker
{
    public static class GpuEcsAnimationBakerMenu
    {
        [MenuItem("Tools/GPU ECS Animation Baker/Add baker component")]
        private static void AddGpuEcsAnimationBaker()
        {
            Selection.activeGameObject.AddComponent<GpuEcsAnimationBakerBehaviour>();
        }    
        
        [MenuItem("Tools/GPU ECS Animation Baker/Add baker component", isValidateFunction:true)]
        private static bool ValidateAddGpuEcsAnimationBaker()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Animator>() != null;
        }
    }
}
#endif