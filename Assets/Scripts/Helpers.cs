using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    private static System.Random rand = new();

    /// <summary>
    /// Calculates arc length given two points on a circle, assumed (start - center).magnitude == (end - center).magnitude
    /// </summary>
    /// <param name="center">The center of the circle.</param>
    /// <param name="a">The first point on the circle.</param>
    /// <param name="b">The second point on the circle.</param>
    /// <returns>The arc length between the two points</returns>
    public static float ArcLength(Vector2 center, Vector2 a, Vector2 b)
    {
        if ((a - center).magnitude - (b - center).magnitude > 0.01f)
        {
            Debug.LogWarning("Points on circle in ArcLength not equidistance from center");
        }

        Vector3 startOffset = a - center;
        Vector3 endOffset = b - center;
        float radiansBetween = Mathf.Deg2Rad * Vector3.Angle(startOffset, endOffset);
        float radius = (a - center).magnitude;

        return radiansBetween * radius;
    }

    public static T RandFromList<T>(List<T> list)
    {
        return list[rand.Next(list.Count)];
    }
}
