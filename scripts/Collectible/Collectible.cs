using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Collectible : Area2D
{
	[Export]
	public AnimationPlayer animationPlayer;
	[Export]
	public Area2D damageEmiiter;
	[Export]
	public float damage;
	public float height;
	public float height_speed;
	public Vector2 heading;
	[Export]
	public float speed = 25;
	[Export]
	public Sprite2D skin;
	public Vector2 velocity;
	public float GRAVITY = 600f;
	[Export]
	public float KNOCK_DOWN_FROCE = 120;
	public State currentState = State.FALL;
	public enum State
	{
		FALL, GROUNDED, FLY
	}
	public enum TYPE
	{
		KNIFE, GUN, FOOD
	}
	[Export]
	public TYPE currentType;

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
		damageEmiiter.AreaEntered += OnDamageEmit;
		damageEmiiter.BodyExited += OnWallExited;
	}

	private void OnWallExited(Node2D body)
	{
		QueueFree();
	}

	private void OnDamageEmit(Area2D area)
	{
		if (area is DamageReceiver damageReceiver && currentState == State.FLY)
		{
			damageReceiver.EmitSignal(DamageReceiver.SignalName.DamageReceived,
			damage, heading, 2);
			QueueFree();
		}
	}




	// Called every frame. 'delta' is the elapsed time since the previous frame.

	public override void _Process(double delta)
	{
		FallHandler((float)delta);
		Position += velocity * (float)delta;
		skin.FlipH = velocity.X < 0;

		AnimationHandler();
		skin.Position = Vector2.Up * height;
		damageEmiiter.Position = Vector2.Up * height;
		Monitorable = currentState == State.GROUNDED;
		damageEmiiter.Monitoring = currentState == State.FLY;
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
		if (currentState == State.FLY)
		{
			velocity = heading * speed;
		}
	}
}
