using UnityEngine;
using System.Collections;

public class ExplosionsScaleCurves : MonoBehaviour
{
    public AnimationCurve ScaleCurveX = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve ScaleCurveY = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve ScaleCurveZ = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public Vector3 GraphTimeMultiplier = Vector3.one, GraphScaleMultiplier = Vector3.one;

    private float startTime;
    private Transform t;
    float evalX, evalY, evalZ;

    private void Awake()
    {
        t = transform;
    }

    private void OnEnable()
    {
        startTime = Time.time;
        evalX = 0;
        evalY = 0; 
        evalZ = 0;
    }

    private void Update()
    {
        var time = Time.time - startTime;

        if (time <= GraphTimeMultiplier.x) {
            evalX = ScaleCurveX.Evaluate(time / GraphTimeMultiplier.x) * GraphScaleMultiplier.x;
        }
        if (time <= GraphTimeMultiplier.y) {
            evalY = ScaleCurveY.Evaluate(time / GraphTimeMultiplier.y) * GraphScaleMultiplier.y;
        }
        if (time <= GraphTimeMultiplier.z) {
            evalZ = ScaleCurveZ.Evaluate(time / GraphTimeMultiplier.z) * GraphScaleMultiplier.z;
        }
        t.localScale = new Vector3(evalX, evalY, evalZ);
    }
}