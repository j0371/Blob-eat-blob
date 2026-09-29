using System.Linq;
using BlobEatBlob.Scripts;
using Godot;

namespace BlobEatBlob.GameScripts;

public partial class LungeCooldownBar : ProgressBar
{
    //Node Properties
    private Player _player;

    private readonly StyleBoxFlat _chargeFillStyle = new() { BgColor = Colors.Orange };


    //Methods
    public override void _Ready()
    {
        _player = GetNode<Player>("../../Player");

        MaxValue = 1;
        Step = 0; //default step is 1, which would snap the bar to empty/full
        ShowPercentage = false;
    }

    public override void _Process(double delta)
    {
        Shark shark = IsInstanceValid(_player) ? _player.GetChildren().OfType<Shark>().FirstOrDefault() : null;

        Visible = shark != null;
        if (shark == null) return;

        if (shark.IsChargingLunge)
        {
            AddThemeStyleboxOverride("fill", _chargeFillStyle);
            Value = shark.ChargeFraction; //fills orange as the lunge charges
        }
        else
        {
            RemoveThemeStyleboxOverride("fill");
            Value = 1 - shark.CooldownRemainingFraction; //fills up as it recharges
        }
    }
}
