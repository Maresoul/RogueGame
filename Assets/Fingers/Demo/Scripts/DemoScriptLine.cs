using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace DigitalRubyShared
{
    /// <summary>
    /// Demo showing off how to draw a line with a touch
    /// </summary>
    public class DemoScriptLine : MonoBehaviour
    {
        [Tooltip("Unity line renderer to draw with")]
        public LineRenderer LineRenderer;

        private PanGestureRecognizer gesture;
        private readonly List<Vector2> points = new List<Vector2>();

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
            Vector2 point = Camera.main.ScreenToWorldPoint(new Vector3(gesture.FocusX, gesture.FocusY, 0.0f));
            if (gesture.State == GestureRecognizerState.Began ||
                gesture.State == GestureRecognizerState.Executing ||
                gesture.State == GestureRecognizerState.Ended)
            {
                if (LineRenderer != null)
                {
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
                            LineRenderer.SetPosition(LineRenderer.positionCount++, new Vector3(points[0].x, points[0].y, 0.0f));
                            for (float t = 0.25f; t <= 1.0f; t += 0.25f)
                            {
                                Vector2 prev = GetBezierPoint(t, points[0], points[1], points[2], points[3]);
                                LineRenderer.SetPosition(LineRenderer.positionCount++, new Vector3(prev.x, prev.y, 0.0f));
                            }
                        }
                        else if (gesture.State == GestureRecognizerState.Ended)
                        {
                            // last point
                            LineRenderer.SetPosition(LineRenderer.positionCount++, new Vector3(points[3].x, points[3].y, 0.0f));
                        }
                        else
                        {
                            // next point
                            Vector2 prev = GetBezierPoint(0.875f, points[0], points[1], points[2], points[3]);
                            LineRenderer.SetPosition(LineRenderer.positionCount++, new Vector3(prev.x, prev.y, 0.0f));
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

        private static Vector2 GetBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float cx = 3 * (p1.x - p0.x);
            float cy = 3 * (p1.y - p0.y);
            float bx = 3 * (p2.x - p1.x) - cx;
            float by = 3 * (p2.y - p1.y) - cy;
            float ax = p3.x - p0.x - cx - bx;
            float ay = p3.y - p0.y - cy - by;
            float Cube = t * t * t;
            float Square = t * t;

            float resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.x;
            float resY = (ay * Cube) + (by * Square) + (cy * t) + p0.y;

            return new Vector2(resX, resY);
        }
    }
}
