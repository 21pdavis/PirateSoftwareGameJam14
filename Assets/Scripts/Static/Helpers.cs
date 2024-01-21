using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    // TODO: potentially move this to PlayerAnimation for when we need different characters
    public static class PlayerAnimStates
    {
        // Gun States
        public const string idleGun = "idle_gun";
        public const string shootGun = "shoot_gun";
        public const string walkGun = "walk_gun";
        public const string shootWalkGun = "shoot_walk_gun";
        public const string caughtGun = "caught_gun";
        public const string swapGunToGrenade = "swap_gun_to_grenade";

        // Grenade States
        public const string idleGrenade = "idle_grenade";
        public const string shootGrenade = "shoot_grenade";
        public const string walkGrenade = "walk_grenade";
        public const string shootWalkGrenade = "shoot_walk_grenade";
        public const string caughtGrenade = "caught_grenade";
        public const string swapGrenadeToGun = "swap_grenade_to_gun";
    }

    public static Color[] AllColors = new Color[]
    {
        Color.green,
        Color.blue,
        Color.red,
        Color.cyan,
        Color.gray,
        Color.magenta,
        Color.white,
        Color.yellow,
        Color.black
    };

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
