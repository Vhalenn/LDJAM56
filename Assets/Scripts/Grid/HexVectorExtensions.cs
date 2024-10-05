using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexVectorExtensions
{
    public static Vector2 WorldToPlanar(this Vector3 world)
    {
        return new Vector2(world.x, world.z);
    }

    public static Vector3 PlanarToWorld(this Vector2 planar, float y = 0f)
    {
        return new Vector3(planar.x, y, planar.y);
    }

    public static Hex ToHex(this Vector3 world)
    {
        return Hex.FromWorld(world);
    }

    public static Hex ToHex(this Vector2 planar)
    {
        return Hex.FromPlanar(planar);
    }
}
