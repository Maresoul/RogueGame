using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace GPUECSAnimationBaker.Samples.SampleScenes
{
    public class FPSScript : MonoBehaviour
    {
    
        private IEnumerator Start()
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            GUI.depth = 2;
            while (true)
            {
                text.text = Math.Round(1f / Time.unscaledDeltaTime).ToString() + " FPS";
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}