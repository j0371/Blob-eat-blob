using System.Linq;
using BlobEatBlob.Scripts;
using Godot;

namespace BlobEatBlob.GameScripts;

public partial class SizeToNextLevel : ProgressBar
{
	//Node Properties
	private Player _player;

	private Label _levelLabel;

	private readonly StyleBoxFlat _chargeFillStyle = new() { BgColor = Colors.Orange };


	//Methods
	public override void _Ready()
	{
		_player = GetNode<Player>("../../Player");

		MaxValue = 1;
		Step = 0; //default step is 1, which would snap the bar to empty/full
		ShowPercentage = false;

		SetAnchorsPreset(LayoutPreset.BottomLeft);
		OffsetLeft = 20;
		OffsetRight = 220;
		OffsetTop = -40;
		OffsetBottom = -20;

		_levelLabel = new Label
		{
			VerticalAlignment = VerticalAlignment.Center
		};
		AddChild(_levelLabel);
		_levelLabel.SetAnchorsAndOffsetsPreset(LayoutPreset.TopWide);
		_levelLabel.OffsetTop = -24;
		_levelLabel.OffsetBottom = 0;
	}

	public override void _Process(double delta)
	{
		if (!IsInstanceValid(_player)) return;

		Value = _player.LevelProgress;
		_levelLabel.Text = $"Shark Level {_player.Level}";
	}
}
