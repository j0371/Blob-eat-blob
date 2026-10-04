using BlobEatBlob.HelperScripts;
using BlobEatBlob.HelperScripts.ErrorCheckAndHandle;
using Godot;

namespace BlobEatBlob.GameScripts;

public partial class SquarePlayerCamera2d : Camera2D
{
	//Necessary Godot Game Properties
	protected bool IsReady => true;


	//Node Properties
	public Rect2 ViewSquare
	{
		get
		{
			Vector2 halfExtents = GetViewport().GetVisibleRect().Size / 2 / Zoom;
			float side = Mathf.Max(halfExtents.X, halfExtents.Y) * 2;

			return new Rect2(GetScreenCenterPosition() - Vector2.One * side / 2, Vector2.One * side);
		}
	}

	private Vector2 _baseZoom;

	//Other Properties


	//Methods
	public override void _Ready()
	{
		if (!IsReady) return;

		if (GeometryWarning.VectorIsNotUniformWarning(Zoom))
		{
			Zoom = Vector2.One * Zoom.X;
		}

		_baseZoom = Zoom;
	}

	public void NormalizeZoom(int levelStartSize)
	{
		Vector2 targetZoom = _baseZoom * GameConfig.Blob.StartingSize / levelStartSize;

		CreateTween().TweenProperty(this, "zoom", targetZoom, 0.5);
	}
}
