using Godot;
using System;
using System.Collections.Generic;

public partial class Collectible : Area2D
{
	[Export]
	public Sprite2D skin;
	[Export]
	public AnimationPlayer animationPlayer;
	public float speed;
	public float height;
	public float height_speed;

	public float GRAVITY = 600f;
	[Export]
	public float KNOCK_DOWN_FROCE = 120;

	public State currentState = State.FALL;

	public enum State
	{
		FALL, GROUNDED, FLY
	}


	public Dictionary<State, string> animationMaps = new Dictionary<State, string>()
	{
		{State.FALL,"FALL"},
		{State.GROUNDED,"GROUNDED"},
		{State.FLY,"FLY"},
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		height_speed = KNOCK_DOWN_FROCE;

	}




	// Called every frame. 'delta' is the elapsed time since the previous frame.

	public override void _Process(double delta)
	{
		FallHandler((float)delta);
		AnimationHandler();
		skin.Position = Vector2.Up * height;
	}

	private void AnimationHandler()
	{
		animationPlayer.Play(animationMaps[currentState]);
	}

	private void FallHandler(float delta)
	{
		if (currentState == State.FALL)
		{
			height += height_speed * delta;
			if (height < 0)
			{
				height = 0;
				currentState = State.GROUNDED;
			}
			height_speed -= GRAVITY * delta;
		}
	}

}
