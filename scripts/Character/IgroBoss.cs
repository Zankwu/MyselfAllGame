using Godot;
using System;

public partial class IgroBoss : Character
{
	[Export]
	public float Boss_Player_Dest_Distance = 30;
	[Export]
	public ulong Time_Bettwen_Attacks_duration = 2000;

	[Export]
	public ulong Time_Recover_duration = 2000;
	public ulong Time_Recover_start = Time.GetTicksMsec();
	public ulong Time_Bettwen_Attacks_Last = Time.GetTicksMsec();


	public Vector2 KNOCKBACK_FORCE;

	public float TO_ZERO_WEIGHT = 50;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public Vector2 GetTargetDistance()
	{
		//BOSS目标点，距离玩家位置30px
		Vector2 target = Vector2.Zero;
		if (player.Position.X > Position.X)
		{
			target = player.Position + (Vector2.Left * Boss_Player_Dest_Distance);
		}
		else
		{
			target = player.Position + (Vector2.Right * Boss_Player_Dest_Distance);
		}
		return target;
	}
	public bool IsPlayerInRange()
	{
		//BOSS是否到达了，目标距离位置
		return (Position - GetTargetDistance()).Length() < 1;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{


		base._Process(delta);
		KNOCKBACK_FORCE = KNOCKBACK_FORCE.MoveToward(Vector2.Zero, (float)delta * TO_ZERO_WEIGHT);
		GD.Print($"KNOCKBACK_FORCE{KNOCKBACK_FORCE}");
	}
	public override void GroundedHandler()
	{
		//BOSS受伤逻辑，只有在RECOVER的状态下才能受伤，此状态会存在2秒
		if (currentState == State.GROUNDED)
		{
			if (current_health > 0)
			{
				currentState = State.RECOVER;
				Time_Recover_start = Time.GetTicksMsec();

			}
			else
			{
				currentState = State.DEATH;
				EntityManager.instance.EmitSignal(EntityManager.SignalName.OnEnemyDeath, this);

			}
		}
		else if (currentState == State.RECOVER)
		{
			if (Time.GetTicksMsec() - Time_Recover_start > Time_Recover_duration)
			{
				currentState = State.IDLE;
				Time_Bettwen_Attacks_Last = Time.GetTicksMsec();
			}
		}
	}

	public override void InputHandler()
	{
		if (player != null && CanMove())
		{
			if (CanPunch() && rayCast2D.IsColliding())
			{
				currentState = State.FLY;
				Velocity = heading * FLY_FORCE;
			}
			else
			{
				if (IsPlayerInRange())
				{
					Velocity = Vector2.Zero;
					currentState = State.IDLE;

				}
				else
				{
					var target = GetTargetDistance();
					var direction = (target - Position).Normalized();
					Velocity = (direction + KNOCKBACK_FORCE) * speed;
					currentState = State.WALK;
				}
			}

		}

	}
	public override void HeadingHandler()
	{
		if (currentState != State.FLY)
		{
			if ((player.Position.X - Position.X) > 0)
			{
				heading = Vector2.Right;
			}
			else
			{
				heading = Vector2.Left;
			}
			base.HeadingHandler();
		}

	}

	public override bool CanGetHurt()
	{
		return true;
	}
	public override bool CanPunch()
	{
		if (Time.GetTicksMsec() - Time_Bettwen_Attacks_Last < Time_Bettwen_Attacks_duration)
		{
			return false;
		}
		return base.CanPunch();
	}
	public override void OnActionComplete()
	{
		if (currentState == State.HURT)
		{
			currentState = State.RECOVER;
			return;
		}
	}


	public bool IsCanBeHurt()
	{
		return currentState == State.RECOVER;
	}
	public override bool IsAttacking()
	{
		if (currentState == State.FLY)
		{
			return true;
		}
		return base.IsAttacking();
	}

	public override void OnDamageReceiver(int damage, Vector2 direction, int HitType)
	{
		if (!IsCanBeHurt())
		{
			KNOCKBACK_FORCE = direction * KNOCK_BACK_FORCE;
			return;
		}
		current_health = Mathf.Clamp(current_health - damage, 0, max_health);
		if (current_health <= 0)
		{
			currentState = State.FALL;
			height_speed = KNOCK_DOWN_FORCE;
			Velocity = direction * KNOCK_DOWN_FORCE;
		}
		else
		{
			currentState = State.HURT;

		}
	}
	public override void OnDamageEmit(Area2D area)
	{
		if (area is DamageReceiver receiver)
		{
			receiver.EmitSignal(DamageReceiver.SignalName.DamageReceived,
			4, heading, 2
			);
			Time_Bettwen_Attacks_Last = Time.GetTicksMsec();
			currentState = State.IDLE;
		}
	}
}
