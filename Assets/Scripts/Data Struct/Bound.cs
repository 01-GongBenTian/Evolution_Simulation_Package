using System;
using System.Collections.Generic;
using UnityEngine;

public class Bound
{
    public Vector2 Min;
    public Vector2 Max;

    public float Width;
    public float Height;

    public bool Within(Bound outer)
    {
        if (outer.Min.x > Min.x || outer.Min.y > Min.y)
        {
            return false;
        }

        if (outer.Max.x < Max.x || outer.Max.y < Max.y)
        {
            return false;
        }

        return true;
    }
}
