using Godot;
using Godot.Collections;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Door : Node2D
{
	[Signal]
	public delegate void OpenedEventHandler();

	public int Door_Index;
	[Export]
	public Sprite2D sprite2D;
	//状态
	public enum State { CLOSED, OPENING, OPENED }

	public State currentState = State.CLOSED;

	public float Open_Start_Time = Time.GetTicksMsec();
	[Export]
	public float Open_Duration_Time = 1000;

	[Export]
	public Array<BasicEnemy> basicEnemies;
	//目标高度
	public int Door_height;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Door_height = sprite2D.Texture.GetHeight();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (currentState == State.OPENING)
		{
			if (Time.GetTicksMsec() - Open_Start_Time > Open_Duration_Time)
			{
				currentState = State.OPENED;
				sprite2D.Position = Vector2.Up * Door_height;
				EmitSignal(SignalName.Opened);
			}
			else
			{
				float progress = (Time.GetTicksMsec() - Open_Start_Time) / Open_Duration_Time;
				sprite2D.Position = Vector2.Zero.Lerp(Vector2.Up * Door_height, progress);
			}
		}
	}

	public void Open()
	{
		if (currentState == State.CLOSED)
		{
			currentState = State.OPENING;
			Open_Start_Time = Time.GetTicksMsec();
		}
	}


}
