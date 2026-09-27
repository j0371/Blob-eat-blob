using Godot;
using System;

namespace BlobEatBlob.GameScripts;

public partial class SquarePlayerCamera2d : Camera2D
{
	//Necessary Godot Game Properties
	protected bool IsReady => true;

	//private isSquare => Camera.Zoom.X == Camera.Zoom.Y
	//Node Properties


	//Other Properties


	//Methods
	public override void _Ready()
	{
		if (!IsReady) return;
	}
}
