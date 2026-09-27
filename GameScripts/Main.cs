using Godot;

namespace BlobEatBlob.Scripts;

public partial class Main : Node2D
{
    //Necessary Godot Game Properties
    protected bool IsReady => true;


    //Node Properties


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;
    }
}
