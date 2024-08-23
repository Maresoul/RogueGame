//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
    /// <summary>
    /// Pan demo script
    /// </summary>
    public class DemoScriptTwoFingerPanScale : MonoBehaviour
    {
        /// <summary>
        /// Transform to modify by gestures
        /// </summary>
        public Transform TransformToModify;

        /// <summary>
        /// Scale speed
        /// </summary>
        [Tooltip("Scale speed")]
        [Range(1.0f, 10.0f)]
        public float ScaleSpeed = 3.0f;

        /// <summary>
        /// Gesture execution
        /// </summary>
        /// <param name="gesture">Gesture</param>
        public void GestureUpdated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                var s = gesture as ScaleGestureRecognizer;
                if (s is null)
                {
                    var p = gesture as PanGestureRecognizer;
                    if (!(p is null))
                    {
                        var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(p.FocusX, p.FocusY, 0.0f));
                        worldPos.z = TransformToModify.position.z;
                        TransformToModify.position = worldPos;
                    }
                }
                else
                {
                    s.ZoomSpeed = ScaleSpeed;
                    TransformToModify.localScale *= s.ScaleMultiplier;
                }
            }
        }
    }
}