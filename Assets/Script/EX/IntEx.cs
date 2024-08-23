using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IntEx
{
    public static int Range(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

    public static bool InRange(this int x)
    {
        if (x <= 0) return false;
        return UnityEngine.Random.Range(0, 101) <= x;
    }


    public static float Range(float min, float max)
    {
        return UnityEngine.Random.Range(min, max + 1);
    }

}
