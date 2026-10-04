using BlobEatBlob.GameScripts;
using BlobEatBlob.HelperScripts;
using Godot;

namespace BlobEatBlob.Scripts;

public partial class Main : Node2D
{
    //Necessary Godot Game Properties
    [Export]
    private Player _player;

    protected bool IsReady => true;


    //Node Properties

    private bool isFullscreen = true;


    //Other Properties


    //Methods
    public override void _Ready()
    {
        if (!IsReady) return;

        _player.Eaten += OnPlayerEaten;
        _player.LeveledUp += ShowLevelTutorial;

        ShowLevelTutorial(_player.Level); //the starting level's tutorial on first launch (already-shown ones are skipped after respawns)
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed(GameConfig.InputActions.Fullscreen))
        {
            if (isFullscreen) DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
            else DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);

            isFullscreen = !isFullscreen;
        }
    }


    private void OnPlayerEaten()
    {
        GetTree().CallDeferred(SceneTree.MethodName.ReloadCurrentScene);
    }

    private void ShowLevelTutorial(int level)
    {
        if (GameConfig.Tutorials.ByLevel.TryGetValue(level, out string text))
            TutorialModal.ShowOnce(this, $"level{level}", text);
    }
}
