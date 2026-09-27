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
        if (enclosedRect.Encloses(enclosingRect)) return false;

        GD.PushError($"Rect {enclosedRectName} is not enclosed by {enclosingRectName}");
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
