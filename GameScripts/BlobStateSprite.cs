using Blobeatblob.Enums.GameStates;
using BlobEatBlob.HelperScripts.ErrorCheckAndHandle;
using BlobEatBlob.Scripts;
using Godot;

namespace BlobEatBlob.GameScripts;

//Swaps the blob's texture so its BlobState is visible: green = normal, red = suppressed, yellow = protected
public partial class BlobStateSprite : Sprite2D
{
    //Necessary Godot Game Properties
    [Export]
    private Texture2D _normalTexture;

    [Export]
    private Texture2D _suppressedTexture;

    [Export]
    private Texture2D _protectedTexture;

    protected bool IsReady => !(this.RequiredGamePropertyNull(_normalTexture)
        | this.RequiredGamePropertyNull(_suppressedTexture)
        | this.RequiredGamePropertyNull(_protectedTexture));


    //Node Properties
    private Blob OwnerBlob => GetParent<Blob>();


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        OwnerBlob.StateChanged += OnBlobStateChanged;
        OnBlobStateChanged(OwnerBlob.BlobState);
    }

    private void OnBlobStateChanged(BlobStates newState)
    {
        Texture = newState switch
        {
            BlobStates.Suppressed => _suppressedTexture,
            BlobStates.Protected => _protectedTexture,
            _ => _normalTexture
        };
    }
}
