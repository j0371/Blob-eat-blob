using System.Runtime.CompilerServices;
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
    public static bool RectangleIsNotASquareWarning(RectangleShape2D rectangle,
        [CallerArgumentExpression(nameof(rectangle))] string rectangleName = "")
    {
        if (Mathf.IsEqualApprox(rectangle.Size.X, rectangle.Size.Y)) return false;

        GD.PushWarning($"Rect {rectangleName} is not a square.");
        return true;
    }

    public static bool VectorIsNotUniformWarning(Vector2 vector,
        [CallerArgumentExpression(nameof(vector))] string vectorName = "")
    {
        if (Mathf.IsEqualApprox(vector.X, vector.Y)) return false;

        GD.PushWarning($"Vector {vectorName} is not uniform.");
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
