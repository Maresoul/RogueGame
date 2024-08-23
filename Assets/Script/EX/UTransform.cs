using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UTransform 
{
    public static void LookTarget(this Transform t,Transform target) {
        if (target==null)
        {
            return;
        }
        Vector3 targetPosition = new Vector3(target.position.x, t.position.y, target.position.z);
        t.LookAt(targetPosition);
    }

    public static void LookTarget(this Transform t, Vector3 target)
    {
        Vector3 targetPosition = new Vector3(target.x, t.position.y, target.z);
        t.LookAt(targetPosition);
    }

    /// <summary>
    /// 获取物体按角度和半径偏移的位置
    /// </summary>
    /// <param name="radius">半径</param>
    /// <param name="angle">角度</param>
    /// <returns></returns>
    public static Vector3 GetOffsetPoint(this Transform t, float radius, float angle)
    {
        if (radius == 0 && angle == 0)
        {
            return t.transform.position;
        }
        float x = Mathf.Sin(angle * Mathf.PI / 180) * radius;
        float z = Mathf.Cos(angle * Mathf.PI / 180) * radius;
        Vector3 end = t.position + t.rotation * new Vector3(x, 0, z);
        return end;
    }

    /// <summary>
    /// 获取物体按角度和半径偏移的位置
    /// </summary>
    /// <param name="radius">半径</param>
    /// <param name="angle">角度</param>
    /// <returns></returns>
    public static Vector3 GetOffsetPoint(this Vector3 position, Quaternion rotation, float radius, float angle)
    {
        if (radius == 0 && angle == 0)
        {
            return position;
        }
        float x = Mathf.Sin(angle * Mathf.PI / 180) * radius;
        float z = Mathf.Cos(angle * Mathf.PI / 180) * radius;
        Vector3 end = position + rotation * new Vector3(x, 0, z);
        return end;
    }

    /// <summary>
    /// 检测在前后
    /// </summary>
    /// <param name="t"></param>
    /// <param name="targetPosition"></param>
    /// <returns>大于0在前方 等于0平行 小于0在后方</returns>
    public static float ForwardOrBack(this Transform t, Vector3 targetPosition)
    {
        //var t2 = targetPosition;
        //t2.y = t.transform.position.y;

        Vector3 delta = new Vector3(targetPosition.x, t.position.y, targetPosition.z) - t.position;
        float v = Vector3.Dot(t.forward, delta);
        return v;//大于0在前方 等于0 平行 小于0在后方
    }

    public static float ForwardOrBack(this Vector3 t, Vector3 targetPosition, Vector3 forward)
    {
        //var t2 = targetPosition;
        //t2.y = t.transform.position.y;

        Vector3 delta = new Vector3(targetPosition.x, t.y, targetPosition.z) - t;
        float v = Vector3.Dot(forward, delta);
        return v;//大于0在前方 等于0 平行 小于0在后方
    }

}
