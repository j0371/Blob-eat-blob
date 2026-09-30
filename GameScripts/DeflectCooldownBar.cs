using System.Linq;
using BlobEatBlob.Scripts;
using Godot;

namespace BlobEatBlob.GameScripts;

public partial class DeflectCooldownBar : ProgressBar
{
    //Node Properties
    private Player _player;

    private readonly StyleBoxFlat _fillStyle = new() { BgColor = DeflectRing.DeflectColor };


    //Methods
    public override void _Ready()
    {
        _player = GetNode<Player>("../../Player");

        MaxValue = 1;
        Step = 0; //default step is 1, which would snap the bar to empty/full
        ShowPercentage = false;
        AddThemeStyleboxOverride("fill", _fillStyle); //same color as the deflect ring
    }

    public override void _Process(double delta)
    {
        Shark shark = IsInstanceValid(_player) ? _player.GetChildren().OfType<Shark>().FirstOrDefault() : null;

        Visible = shark != null;
        if (shark == null) return;

        Value = 1 - shark.DeflectCooldownRemainingFraction; //fills up as it recharges
    }
}
