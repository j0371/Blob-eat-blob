using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace BlobEatBlob.HelperScripts.ErrorCheckAndHandle;

public static class GeometryError
{
    public static bool RectNotEnclosedError(Rect2 enclosedRect, Rect2 enclosingRect,
        [CallerArgumentExpression(nameof(enclosedRect))] string enclosedRectName = "",
        [CallerArgumentExpression(nameof(enclosingRect))] string enclosingRectName = "")
    {
        if (enclosingRect.Encloses(enclosedRect)) return false;

        GD.PushError($"Rect {enclosedRectName} is not enclosed by {enclosingRectName}");
        return true;
    }


}

public static class GeometryWarning
{
        public static bool RectangleIsNotASquareWarning(Rect2 rect,
        [CallerArgumentExpression(nameof(rect))] string rectName = "")
    {
        if (Mathf.IsEqualApprox(rect.Size.X, rect.Size.Y)) return false;

        GD.PushError($"Rect {rect} is not a square.");
        return true;
    }

    public static bool RectangleIsNotASquareWarning(RectangleShape2D rectangle,
        [CallerArgumentExpression(nameof(rectangle))] string rectangleName = "")
    {
        if (Mathf.IsEqualApprox(rectangle.Size.X, rectangle.Size.Y)) return false;

        GD.PushError($"Rect {rectangleName} is not a square.");
        return true;
    }
    
}

public static class Node2DErrorExtensions
{
    public static bool RequiredGamePropertyNull(this Node node, GodotObject value,
        [CallerArgumentExpression(nameof(value))]
        string name = "")
    {
        if (value is not null) return false;
        GD.PushError($"{name} is not assigned.");
        node.SetProcess(false);
        node.SetPhysicsProcess(false);
        return true;
    }
}
