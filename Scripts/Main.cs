
using System.Collections.Generic;
using Godot;

public partial class Main : Node3D
{

	Camera3D mainCamera;

	int score = 0;
	bool dragging = false;

	Label scoreLabel;

	GameController gameController;

	public override void _Ready()
	{
		mainCamera = GetNode<Camera3D>("Camera3D");
		Rect2 viewport = GetViewport().GetVisibleRect();
		var minPOs = mainCamera.ProjectPosition(viewport.Position, 1f);
		var maxPos = mainCamera.ProjectPosition(viewport.Size, 1f);
		float width = Mathf.Abs(maxPos.X - minPOs.X);
		float height = Mathf.Abs(maxPos.Y - minPOs.Y);
		GD.Print("MinPos: " + minPOs + " MaxPos: " + maxPos + " Viewport size: " + viewport.Size + " Viewport position: " + viewport.Position);
		gameController = new GameController(height, width, mainCamera);
		gameController.LoadWorld(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// remove activeItems that have been dropped
		// toPlayItems.Clear();
		// scoreLabel.Text = "Score: " + score;

	}

}
