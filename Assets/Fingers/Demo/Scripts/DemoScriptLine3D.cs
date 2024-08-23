using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DigitalRubyShared
{
    /// <summary>
    /// Demo showing off how to draw a line with a touch on a 3d surface
    /// </summary>
    public class DemoScriptLine3D : MonoBehaviour
    {
        [Tooltip("Unity line renderer to draw with")]
        public LineRenderer LineRenderer;

        [Tooltip("Target to draw on")]
        public Transform Target;
        
        [Tooltip("Distance in world units to offset line from object being drawn on")]
        [Range(0.01f, 1.0f)]
        public float OffsetFromObject = 0.2f;
        
        private PanGestureRecognizer gesture;
        private readonly List<Vector3> points = new List<Vector3>();

        private void OnEnable()
        {
            gesture = new PanGestureRecognizer();
            gesture.StateUpdated += Gesture_Updated;
            gesture.ThresholdUnits = 0; // start gesture immediately
            FingersScript.Instance.AddGesture(gesture);
        }

        private void OnDisable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(gesture);
            }
        }

        private void Gesture_Updated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Began ||
                gesture.State == GestureRecognizerState.Executing ||
                gesture.State == GestureRecognizerState.Ended)
            {
                if (LineRenderer != null)
                {
                    var ray = Camera.main.ScreenPointToRay(new Vector3(gesture.FocusX, gesture.FocusY, Camera.main.transform.position.z));
                    if (!Physics.Raycast(ray, out RaycastHit hitInfo) || hitInfo.transform != Target)
                    {
                        return;
                    }
                    Vector3 point = hitInfo.point + (hitInfo.normal * OffsetFromObject);
                    point = LineRenderer.transform.InverseTransformPoint(point);
                    if (gesture.State == GestureRecognizerState.Began)
                    {
                        points.Clear();
                        LineRenderer.positionCount = 0;
                    }
                    if (points.Count == 0 || Mathf.Abs(gesture.DeltaX) > 0.5f || Mathf.Abs(gesture.DeltaY) > 0.5f)
                    {
                        points.Add(point);
                    }
                    if (points.Count == 4)
                    {
                        if (LineRenderer.positionCount == 0)
                        {
                            // first set of points
                            LineRenderer.SetPosition(LineRenderer.positionCount++, points[0]);
                            for (float t = 0.25f; t <= 1.0f; t += 0.25f)
                            {
                                Vector3 prev = GetBezierPoint(t, points[0], points[1], points[2], points[3]);
                                LineRenderer.SetPosition(LineRenderer.positionCount++, prev);
                            }
                        }
                        else if (gesture.State == GestureRecognizerState.Ended)
                        {
                            // last point
                            LineRenderer.SetPosition(LineRenderer.positionCount++, points[3]);
                        }
                        else
                        {
                            // next point
                            Vector3 prev = GetBezierPoint(0.875f, points[0], points[1], points[2], points[3]);
                            LineRenderer.SetPosition(LineRenderer.positionCount++, prev);
                        }
                        points.RemoveAt(0);
                    }
                }
                else
                {
                    Debug.LogError("LineRenderer is null on demo script line.");
                }
            }
        }

        private static Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;

            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;
            return p;

        }
    }
}
