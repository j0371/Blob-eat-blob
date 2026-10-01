using Godot;

namespace BlobEatBlob.Scripts;

public partial class Main : Node2D
{
    //Necessary Godot Game Properties
    [Export]
    private Player _player;

    protected bool IsReady => true;


    //Node Properties


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        _player.Eaten += OnPlayerEaten;
    }

    private void OnPlayerEaten()
    {
        GetTree().CallDeferred(SceneTree.MethodName.ReloadCurrentScene);
    }
}
