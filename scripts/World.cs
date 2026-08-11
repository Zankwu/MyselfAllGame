using Godot;
using System;

public partial class World : Node2D
{

	[Export]
	public Camera2D camera;
	[Export]
	public AnimatableBody2D leftWall;
	[Export]
	public AnimatableBody2D rightWall;
	[Export]
	public Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		CameraHandler();
	}

	public void CameraHandler()
	{
		if (camera.Position.X < player.Position.X)
		{
			camera.Position = new Vector2(player.Position.X, camera.Position.Y);
			rightWall.Position = new Vector2(camera.Position.X + (GetViewportRect().Size.X/2),rightWall.GlobalPosition.Y);
		}

	}
}
