using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class BezierCurve
{
    /// <summary>
    /// 二阶贝塞尔曲线
    /// </summary>
    /// <param name="p0">起点</param>
    /// <param name="p1">操作点,用于控制曲线</param>
    /// <param name="p2">终点</param>
    /// <param name="t">0-1 经过的时间/总消耗的时间</param>
    /// <returns>指定时间所处的位置</returns>
    public static Vector3 GetPointOnQuadraticBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }


    /// <summary>
    /// 三阶贝塞尔曲线
    /// </summary>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制曲线点1</param>
    /// <param name="p2">控制曲线点2</param>
    /// <param name="p3">终点</param>
    /// <param name="t">0-1 经过的时间/总消耗的时间</param>
    /// <returns>指定时间所处的位置</returns>
    public static Vector3 GetPointOnCubicBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = (uuu * p0) + (3 * uu * t * p1) + (3 * u * tt * p2) + (ttt * p3);
        return p;
    }
    //(B(t) = (1-t)^2*P_0 + 2t(1-t)*P_1 + t^2*P_2)
    //(B(t) = (1-t)^3*P_0 + 3t(1-t)^2*P_1 + 3t^2(1-t)*P_2 + t^3*P_3)

}
