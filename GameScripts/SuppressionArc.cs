using Blobeatblob.Enums.GameStates;
using BlobEatBlob.Scripts;
using Godot;

namespace BlobEatBlob.GameScripts;

//Arc on the blob's rim that drains as its suppression runs out
public partial class SuppressionArc : Node2D
{
    //Necessary Godot Game Properties
    protected bool IsReady => true;


    //Node Properties
    private Blob OwnerBlob => GetParent<Blob>();

    //In the blob's local units, just inside the sprite circle (radius ~10)
    [Export]
    private float _radius = 8f;

    [Export]
    private float _width = 2f;


    //Other Properties
    private static readonly Color ArcColor = new(1, 1, 1, 0.8f);


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        OwnerBlob.StateChanged += OnBlobStateChanged;
        OnBlobStateChanged(OwnerBlob.BlobState);
    }

    private void OnBlobStateChanged(BlobStates newState)
    {
        //only processes while suppressed, so the other blobs cost nothing per frame
        bool isSuppressed = newState is BlobStates.Suppressed;
        Visible = isSuppressed;
        SetProcess(isSuppressed);
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        float fraction = (float)OwnerBlob.SuppressionRemainingFraction;
        if (fraction <= 0) return;

        //starts at the top and drains clockwise
        float start = -Mathf.Pi / 2;
        DrawArc(Vector2.Zero, _radius, start, start + Mathf.Tau * fraction, 48, ArcColor, _width, true);
    }
}
