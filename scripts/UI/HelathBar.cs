using Godot;
using System;

public partial class HelathBar : Control
{

	[Export]
	public ColorRect whiteBorder;
	[Export]
	public ColorRect redBorder;
	[Export]
	public TextureRect healthRect;

	[Export]
	public bool isInvert;
	// public int max_health;
	// public int current_health;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void Refresh(int max_health, int current_health)
	{
		// int i = 0;
		int i = isInvert ? -1 : 1;
		whiteBorder.Scale = new Vector2((max_health + 2) * i, whiteBorder.Scale.Y);
		redBorder.Scale = new Vector2(max_health * i, redBorder.Scale.Y);
		healthRect.Scale = new Vector2(current_health * i, healthRect.Scale.Y);

	}
}
