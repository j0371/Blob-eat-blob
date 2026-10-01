using BlobEatBlob.Scripts;
using Godot;

namespace BlobEatBlob.GameScripts;

//Arrow outside the blob pointing where a lunge would go. Dim when a lunge can't start, orange and stretching while charging
public partial class AimArrow : Node2D
{
    //Necessary Godot Game Properties
    protected bool IsReady => true;


    //Node Properties
    private Shark OwnerShark => GetParent<Shark>();

    private Blob OwnerBlob => OwnerShark.GetParent<Blob>();

    //Sizes are in the blob's local units, so the arrow grows with the blob (sprite circle radius is ~10)
    [Export]
    private float _distanceFromCenter = 13f;

    [Export]
    private float _length = 5f;

    [Export]
    private float _halfWidth = 3f;

    [Export]
    private float _maxChargeExtraLength = 5f;


    //Other Properties
    private static readonly Color ReadyColor = Colors.White;

    private static readonly Color UnavailableColor = new(1, 1, 1, 0.3f);

    private static readonly Color ChargingColor = Colors.Orange;


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
    }

    public override void _Process(double delta)
    {
        GlobalRotation = OwnerBlob.AimDirection.Angle();
        QueueRedraw();
    }

    public override void _Draw()
    {
        Shark shark = OwnerShark;

        float length = _length + _maxChargeExtraLength * (float)shark.ChargeFraction;

        Color color = shark.IsChargingLunge ? ChargingColor
            : shark.CanStartLunge ? ReadyColor
            : UnavailableColor;

        //Drawn pointing along +X; _Process rotates the node toward the aim direction
        DrawColoredPolygon(
        [
            new Vector2(_distanceFromCenter, -_halfWidth),
            new Vector2(_distanceFromCenter + length, 0),
            new Vector2(_distanceFromCenter, _halfWidth),
        ], color);
    }
}
