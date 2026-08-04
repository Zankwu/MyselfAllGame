using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Character : CharacterBody2D
{
	[Export]
	public bool can_respawn;

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
	public CollisionShape2D collisionShape2D;

	[Export]
	public DamageEmitter damageEmitter;
	[Export]
	public DamageEmitter chainReactionEmit;
	[Export]
	public DamageReceiver damageReceiver;

	[Export]
	public float FLY_FORCE = 100;
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
	// 1 = NORMAL 2 = JUMPKICK 3 = POWER
	public int HitType;
	public float height_speed = 0f;
	public State currentState = State.IDLE;
	public Vector2 heading = Vector2.Right;

	public bool is_last_attack_sucessful;
	public enum State
	{
		IDLE, WALK, ATTACK, TAKEOFF, JUMP, LAND, JUMPKICK,
		HURT, FALL, GROUNDED, DEATH, FLY,PREP_PUNCH
	}

	public Dictionary<State, string> animationMap = new Dictionary<State, string>()
	{
		{State.IDLE,"IDLE"},
		{State.WALK,"WALK"},
		{State.ATTACK,"ATTACK"},
		{State.TAKEOFF,"TAKEOFF"},
		{State.JUMP,"JUMP"},
		{State.LAND,"LAND"},
		{State.JUMPKICK,"JUMPKICK"},
		{State.HURT,"HURT"},
		{State.FALL,"FALL"},
		{State.GROUNDED,"GROUNDED"},
		{State.DEATH,"GROUNDED"},
		{State.FLY,"FLY"},
		{State.PREP_PUNCH,"IDLE"},
	};
	public string[] attack_animations ;

	public int attack_combo_index = 0;

	[Export]
	public ulong Time_Grounded_Duration = 1000;

	public ulong Time_Grounded_Start = Time.GetTicksMsec();
	public ulong Time_Death_Start = Time.GetTicksMsec();



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		damageEmitter.AreaEntered += OnDamageEmit;
		damageReceiver.DamageReceived += OnDamageReceiver;
		chainReactionEmit.BodyEntered += OnWallHit;
		chainReactionEmit.AreaEntered += OnEnemyChainReaction;
		current_health = max_health;
	}



	public override void _Process(double delta)
	{
		AnimationHandler();
		AirTimeHandler((float)delta);
		DeahtHandler(delta);
		HeadingHandler();
		InputHandler();
		GroundedHandler();
		MoveHandler();
		FlipScale();
		PrepAttackHandler();
		MoveAndSlide();


		if (labelState != null)
		{
			labelState.Text = animationMap[currentState];

		}
		damageEmitter.Monitoring = currentState == State.ATTACK || currentState == State.JUMPKICK;
		collisionShape2D.Disabled = IsCollisionShape2DEnable();
		skin.Position = Vector2.Up * height;

		chainReactionEmit.Monitoring = currentState == State.FLY;

	}

	public virtual void PrepAttackHandler()
	{
	}

	private bool IsCollisionShape2DEnable()
	{
		return currentState == State.GROUNDED || currentState == State.DEATH
		|| currentState == State.FLY;
	}

	private void DeahtHandler(double delta)
	{
		if (currentState == State.DEATH)
		{
			Color modulateA = skin.Modulate;
			modulateA.A -= (float)delta;
			skin.Modulate = modulateA;
			if (skin.Modulate.A <= 0)
			{
				QueueFree();
			}
		}

	}

	public virtual void HeadingHandler()
	{

	}
	public void FlipScale()
	{
		if (heading == Vector2.Right)
		{
			damageEmitter.Scale = new Vector2(1, damageEmitter.Scale.Y);
			skin.FlipH = false;
		}
		else if (heading == Vector2.Left)
		{
			damageEmitter.Scale = new Vector2(-1, damageEmitter.Scale.Y);
			skin.FlipH = true;
		}
	}
	public virtual void AnimationHandler()
	{
		if (currentState == State.ATTACK)
		{
			animation.Play(attack_animations[attack_combo_index]);
		}
		else
		{
			animation.Play(animationMap[currentState]);

		}
	}

	public virtual void GroundedHandler()
	{
		if (currentState == State.GROUNDED && Time.GetTicksMsec() - Time_Grounded_Start > Time_Grounded_Duration)
		{
			if (current_health <= 0)
			{
				currentState = State.DEATH;
			}
			else
			{
				currentState = State.LAND;

			}
		}
	}
	public virtual void AirTimeHandler(float delta)
	{
		if (currentState == State.JUMP || currentState == State.JUMPKICK || currentState == State.FALL)
		{
			height += height_speed * delta;

			if (height <= 0)
			{
				height = 0;
				if (currentState == State.FALL)
				{
					currentState = State.GROUNDED;
					Velocity = Vector2.Zero;
					Time_Grounded_Start = Time.GetTicksMsec();

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
	public virtual bool CanPunch()
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

	public bool CanGetHurt()
	{
		return currentState == State.IDLE || currentState == State.WALK
		|| currentState == State.TAKEOFF || currentState == State.LAND
		|| currentState == State.JUMP
		;
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
		is_last_attack_sucessful = true;
		int damage_temp = damage;
		HitType = 1;
		if (currentState == State.JUMPKICK)
		{
			HitType = 2;
		}
		if (attack_combo_index == attack_animations.Length - 1)
		{
			HitType = 3;
			damage_temp *= (int)1.5;
		}
		if (area is DamageReceiver damageReceiverTemp)
		{
			damageReceiverTemp.EmitSignal(DamageReceiver.SignalName.DamageReceived, damage_temp, heading, HitType);
		}

	}

	public virtual void OnDamageReceiver(int damage, Vector2 direction, int HitType)
	{
		if (CanGetHurt())
		{
			current_health = Mathf.Clamp(current_health - damage, 0, max_health);

			if (HitType == 2 || current_health <= 0)
			{
				currentState = State.FALL;
				height_speed = KNOCK_DOWN_FORCE;
				Velocity = direction * KNOCK_BACK_FORCE;

			}
			else if (HitType == 3)
			{
				currentState = State.FLY;
				Velocity = direction * FLY_FORCE;
			}
			else
			{
				currentState = State.HURT;
				Velocity = direction * KNOCK_BACK_FORCE;

			}
		}

	}
	private void OnEnemyChainReaction(Area2D area)
	{
		if (area is DamageReceiver receiver)
		{
			if (receiver != damageReceiver)
			{
				HitType = 2;
				receiver.EmitSignal(DamageReceiver.SignalName.DamageReceived, 0, heading, HitType);
			}
		}
	}

	private void OnWallHit(Node2D body)
	{
		if (currentState == State.FLY)
		{
			currentState = State.FALL;
			height_speed = KNOCK_DOWN_FORCE;
			Velocity = -Velocity / 2;
		}
	}
}
