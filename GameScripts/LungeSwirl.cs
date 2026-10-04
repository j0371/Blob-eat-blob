using Godot;

namespace BlobEatBlob.GameScripts;

//2-frame swirl loop around the blob while it is lunging
public partial class LungeSwirl : AnimatedSprite2D
{
    //Node Properties
    private Shark OwnerShark => GetParent<Shark>();

    private const string FramesFolder = "res://Assets/AI Generated/LungeAnimation/";


    //Methods
    public override void _Ready()
    {
        SpriteFrames = new SpriteFrames(); //comes with an empty looping "default" animation
        SpriteFrames.AddFrame("default", GD.Load<Texture2D>(FramesFolder + "LungeFrameOne.png"));
        SpriteFrames.AddFrame("default", GD.Load<Texture2D>(FramesFolder + "LungeFrameTwo.png"));
        SpriteFrames.SetAnimationSpeed("default", 10); //frames per second

        Scale = Vector2.One * 0.03f; //the images are large; tune until it wraps the blob
        Visible = false;
    }

    public override void _Process(double delta)
    {
        bool isLunging = OwnerShark.IsLunging;

        if (isLunging && !Visible) Play();
        else if (!isLunging && Visible) Stop();

        Visible = isLunging;
    }
}