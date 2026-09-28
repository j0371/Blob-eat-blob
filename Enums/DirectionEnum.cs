using Godot;
using System;

namespace BlobEatBlob.Enums;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
    UpRight,
    UpLeft,
    DownRight,
    DownLeft
}

public static class DirectionExtensions
{
    public static StringName Input(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => "up",
            Direction.Down => "down",
            Direction.Left => "left",
            Direction.Right => "right",
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }

    public static Vector2 Vector(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Vector2.Up,
            Direction.Down => Vector2.Down,
            Direction.Left => Vector2.Left,
            Direction.Right => Vector2.Right,
            Direction.UpRight => Vector2.Up + Vector2.Right,
            Direction.UpLeft => Vector2.Up + Vector2.Left,
            Direction.DownRight => Vector2.Down + Vector2.Right,
            Direction.DownLeft => Vector2.Down + Vector2.Left,
                    _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
}