using Godot;

namespace BlobEatBlob.GameScripts;

//Ring around the blob while it is deflecting: fully on for the whole deflect window, off otherwise
public partial class DeflectRing : Node2D
{
    //Necessary Godot Game Properties
    protected bool IsReady => true;


    //Node Properties
    private Shark OwnerShark => GetParent<Shark>();

    //In the blob's local units, outside the player/size ring (radius 12)
    [Export]
    private float _radius = 14.5f;

    [Export]
    private float _width = 2f;


    //Other Properties
    public static readonly Color DeflectColor = Colors.White;


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
    }

    public override void _Process(double delta)
    {
        Visible = OwnerShark.IsDeflecting;
    }

    public override void _Draw()
    {
        DrawArc(Vector2.Zero, _radius, 0, Mathf.Tau, 48, DeflectColor, _width, true);
    }
}
