using BlobEatBlob.Scripts;
using Godot;

namespace BlobEatBlob.GameScripts;

//Ring showing what a blob is to the player: blue = the player, red = smaller (edible), yellow = bigger (dangerous), none = same size
public partial class BlobRing : Node2D
{
    //Necessary Godot Game Properties
    protected bool IsReady => true;


    //Node Properties
    private Blob OwnerBlob => GetParent<Blob>();

    private Player _player;

    private Color? _currentColor;

    //In the blob's local units, just outside the sprite circle (radius ~10)
    [Export]
    private float _radius = 12f;

    [Export]
    private float _width = 1.5f;


    //Other Properties
    public const string PlayerGroup = "player"; //set on the root node in Player.tscn

    private static readonly Color PlayerColor = new(0.2f, 0.5f, 1f);

    private static readonly Color SmallerColor = Colors.Red;

    private static readonly Color BiggerColor = Colors.Yellow;


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
    }

    public override void _Process(double delta)
    {
        Color? color = GetRingColor();
        if (color == _currentColor) return;

        _currentColor = color;
        QueueRedraw();
    }

    private Color? GetRingColor()
    {
        if (OwnerBlob is Player) return PlayerColor;

        if (!IsInstanceValid(_player)) _player = GetTree().GetFirstNodeInGroup(PlayerGroup) as Player;
        if (_player is null) return null;

        if (OwnerBlob.Size < _player.Size) return SmallerColor;
        if (OwnerBlob.Size > _player.Size) return BiggerColor;
        return null;
    }

    public override void _Draw()
    {
        if (_currentColor is Color color) DrawArc(Vector2.Zero, _radius, 0, Mathf.Tau, 48, color, _width, true);
    }
}
