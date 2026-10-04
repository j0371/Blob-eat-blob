using System.Collections.Generic;
using Godot;

namespace BlobEatBlob.GameScripts;

//Pauses the game and shows a tutorial message until the player presses Start
public partial class TutorialModal : CanvasLayer
{
    //Node Properties
    private static readonly HashSet<string> shownTutorials = new(); //static, so tutorials don't repeat after the scene reloads on death

    public string Text { get; init; }


    //Methods
    public static void ShowOnce(Node parent, string id, string text)
    {
        if (!shownTutorials.Add(id)) return; //Add returns false if this id was already shown

        parent.AddChild(new TutorialModal { Text = text });
    }

    public override void _Ready()
    {
        Layer = 10; //draws above the HUD
        ProcessMode = ProcessModeEnum.Always; //keeps the button working while the game is paused

        //dims the game and blocks clicks from reaching it
        ColorRect dim = new() { Color = new Color(0, 0, 0, 0.6f) };
        AddChild(dim);
        dim.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        //centers the panel on screen
        CenterContainer center = new();
        AddChild(center);
        center.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        PanelContainer panel = new();
        center.AddChild(panel);

        MarginContainer margin = new();
        foreach (string side in new[] { "left", "right", "top", "bottom" })
            margin.AddThemeConstantOverride($"margin_{side}", 24);
        panel.AddChild(margin);

        VBoxContainer content = new();
        content.AddThemeConstantOverride("separation", 16);
        margin.AddChild(content);

        Label text = new()
        {
            Text = Text,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            CustomMinimumSize = new Vector2(500, 0) //wrap width
        };
        content.AddChild(text);

        Button startButton = new() { Text = "Start" };
        startButton.Pressed += Close;
        content.AddChild(startButton);

        startButton.GrabFocus(); //lets Enter/Space close it too
        GetTree().Paused = true;
    }

    private void Close()
    {
        GetTree().Paused = false;
        QueueFree();
    }
}
