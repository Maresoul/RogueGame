using UnityEngine;

namespace GPUECSAnimationBaker.Samples.SampleScenes._1_LODs
{
    public class ZoomScript : MonoBehaviour
    {
        public float minDistance;
        public float maxDistance;
        public void Zoom(float value)
        {
            transform.position = new Vector3( 
                transform.position.x,
                transform.position.y,
                -(minDistance + value * (maxDistance - minDistance)));
        }
    }
}
