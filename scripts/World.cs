using Godot;
using System;

public partial class World : Node2D
{

	[Export]
	public Camera2D camera;

	[Export]
	public Player player;

	public bool is_camera_locked = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EntityManager.instance.CheckPointStart += CheckPointStart;
		EntityManager.instance.CheckPointEnd += CheckPointEnd;
		
	}

	private void CheckPointEnd()
	{
		is_camera_locked = false;
	}


	private void CheckPointStart()
	{
		is_camera_locked = true;
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CameraHandler();
	}

	public void CameraHandler()
	{
		if (camera.Position.X < player.Position.X && !is_camera_locked)
		{
			camera.Position = new Vector2(player.Position.X, camera.Position.Y);
		}

	}
}
