using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum DoorDirection
{
    None,
    North,
    East,
    South,
    West
}

public static class DoorDirectionExtensions
{
    public static Vector2 DirectionToVector2(this DoorDirection doorDirection)
    {
        switch (doorDirection)
        {
            case DoorDirection.North:
                return Vector2.up;
            case DoorDirection.East:
                return Vector2.right;
            case DoorDirection.South:
                return Vector2.down;
            case DoorDirection.West:
                return Vector2.left;
            default: return Vector2.zero;
        }
    }
}
