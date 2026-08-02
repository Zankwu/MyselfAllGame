using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Character : CharacterBody2D
{
	[Export]
	public Label labelState;

	[Export]
	public int max_health;
	public int current_health;

	[Export]
	public int damage;
	[Export]
	public int speed;



	[Export]
	public AnimationPlayer animation;

	[Export]
	public DamageEmitter damageEmitter;
	[Export]
	public DamageReceiver damageReceiver;

	[Export]
	public CollisionShape2D collisionShape2D;
	[Export]
	public Sprite2D skin;

	[Export]
	public StateMachine stateMachine;
	[Export]
	public float KNOCK_BACK_FORCE = 50;
	[Export]
	public float KNOCK_DOWN_FORCE = 70;
	public float GRAVITY = 600f;
	public float JUMPFORCE = 150f;
	public float height = 0f;
	public float height_speed = 0f;
	public State currentState = State.IDLE;
	public bool heading = false;


	// 1 = NORMAL 2 = JUMPKICK 3 = POWER
	public int HitType;
	public enum State
	{
		IDLE, WALK, ATTACK, TAKEOFF, JUMP, LAND, JUMPKICK,
		HURT, FALL, GROUNDED
	}

	public Dictionary<State, string> animationMap = new Dictionary<State, string>()
	{
		{State.IDLE,"IDLE"},
		{State.WALK,"WALK"},
		{State.ATTACK,"PUNCH"},
		{State.TAKEOFF,"TAKEOFF"},
		{State.JUMP,"JUMP"},
		{State.LAND,"LAND"},
		{State.JUMPKICK,"JUMPKICK"},
		{State.HURT,"HURT"},
		{State.FALL,"FALL"},
		{State.GROUNDED,"GROUNDED"},
	};

	[Export]
	public ulong Time_Grounded_duration = 1000;

	public ulong Time_Grounded_start = Time.GetTicksMsec();



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		damageEmitter.AreaEntered += OnDamageEmit;
		damageReceiver.DamageReceived += OnDamageReceiver;
		current_health = max_health;
		collisionShape2D.Disabled = currentState == State.GROUNDED;
	}


	public override void _Process(double delta)
	{
		MoveHandler();
		AnimationHandler();
		HeadingHandler();
		InputHandler();
		AirTimeHandler((float)delta);
		GroundedHandler();
		MoveAndSlide();
		if (labelState != null)
		{
			labelState.Text = animationMap[currentState];

		}
		damageEmitter.Monitoring = currentState == State.ATTACK || currentState == State.JUMPKICK;
		skin.Position = Vector2.Up * height;

		skin.FlipH = heading;


	}

	public virtual void GroundedHandler()
	{
		if (currentState == State.GROUNDED && Time.GetTicksMsec() - Time_Grounded_start > Time_Grounded_duration)
		{
			currentState = State.LAND;
		}
	}
	public virtual void HeadingHandler()
	{


		if (!heading)
		{

			Vector2 s = new Vector2(1, damageEmitter.Scale.Y);
			damageEmitter.Scale = s;
		}
		else if (heading)
		{

			Vector2 s = new Vector2(-1, damageEmitter.Scale.Y);
			damageEmitter.Scale = s;
		}


	}

	public virtual void AnimationHandler()
	{
		animation.Play(animationMap[currentState]);

	}

	public virtual void AirTimeHandler(float delta)
	{
		if (currentState == State.JUMP || currentState == State.JUMPKICK || currentState == State.FALL)
		{
			height += height_speed * delta;
			if (height < 0)
			{
				height = 0;
				if (currentState == State.FALL)
				{
					currentState = State.GROUNDED;
					Velocity = Vector2.Zero;
					Time_Grounded_start = Time.GetTicksMsec();
				}
				else
				{
					currentState = State.LAND;

				}
			}
			height_speed -= GRAVITY * delta;
		}
	}

	public virtual void MoveHandler()
	{
		if (CanMove())
		{
			if (Velocity == Vector2.Zero)
			{
				currentState = State.IDLE;
			}
			else
			{
				currentState = State.WALK;
			}
		}
		else if (currentState == State.ATTACK)
		{
			Velocity = Vector2.Zero;
		}

	}

	public virtual void InputHandler()
	{

	}
	public bool CanMove()
	{
		return currentState == State.IDLE || currentState == State.WALK;
	}
	public bool CanPunch()
	{
		return currentState == State.IDLE || currentState == State.WALK;
	}
	public bool CanJump()
	{
		return currentState == State.IDLE || currentState == State.WALK;
	}
	public bool CanJumpKick()
	{
		return currentState == State.JUMP;
	}
	public void OnActionComplete()
	{
		currentState = State.IDLE;
	}
	public void OnTakeOffComplete()
	{
		currentState = State.JUMP;
	}
	public void OnLandComplete()
	{
		currentState = State.IDLE;
	}
	public void OnDamageEmit(Area2D area)
	{
		HitType = 1;

		if (currentState == State.JUMPKICK)
		{
			HitType = 2;
		}
		if (area is DamageReceiver damageReceiverTemp)
		{
			var direction_temp = heading ? Vector2.Left : Vector2.Right;
			damageReceiverTemp.EmitSignal(DamageReceiver.SignalName.DamageReceived, damage, direction_temp, HitType);
		}
	}

	public virtual void OnDamageReceiver(int damage, Vector2 direction, int HitType)
	{
		current_health = Mathf.Clamp(current_health - damage, 0, max_health);


		if (HitType == 2 || current_health <= 0)
		{
			currentState = State.FALL;
			height_speed = KNOCK_DOWN_FORCE;
		}
		else
		{
			currentState = State.HURT;
		}
		Velocity = direction * KNOCK_BACK_FORCE;


	}
}
