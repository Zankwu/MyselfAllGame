using Godot;
using System;

public partial class World : Node2D
{

	[Export]
	public Camera2D camera;

	[Export]
	public RemoteTransform2D remoteTransform2D;
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
		if (remoteTransform2D.Position.X < player.Position.X)
		{
			remoteTransform2D.Position = new Vector2(player.Position.X, remoteTransform2D.Position.Y);

		}

	}
}
