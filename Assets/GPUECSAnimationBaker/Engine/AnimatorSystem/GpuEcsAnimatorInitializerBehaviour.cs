using System;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.AnimatorSystem
{
    [RequireComponent(typeof(GpuEcsAnimatorBehaviour))]
    public class GpuEcsAnimatorInitializerBehaviour : MonoBehaviour
    {
        public virtual int GetInitialAnimationID() { return 0; }
    }

    [RequireComponent(typeof(GpuEcsAnimatorBehaviour))]
    public class GpuEcsAnimatorInitializerBehaviour<T> : GpuEcsAnimatorInitializerBehaviour where T : Enum
    {
        public T initialAnimationId;

        public override int GetInitialAnimationID()
        {
            Array values = Enum.GetValues(typeof(T));
            for (int i = 0; i < values.Length; i++)
                if((values.GetValue(i)).Equals(initialAnimationId)) return i;
            return 0;
        }
    }
    
}