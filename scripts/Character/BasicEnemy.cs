using Godot;
using System;
using System.Linq;

public partial class BasicEnemy : Character
{


	[Export]
	public Player player;

	public EnemySlot enemySlot;

	[Export]
	public ulong TimeLastAttackDuration = 2000;

	[Export]
	public ulong TimePrepAttackDuration = 500;

	public ulong TimePrepAttackStart = Time.GetTicksMsec();
	public ulong TimeLastAttackStart = Time.GetTicksMsec();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		attack_animations = ["PUNCH", "PUNCH_AIT"];
		base._Ready();
	}


	public override void HeadingHandler()
	{

		float temp = GlobalPosition.X - player.GlobalPosition.X;
		if (temp < 0)
		{
			heading = Vector2.Right;
		}
		else
		{
			heading = Vector2.Left;
		}
		base.HeadingHandler();
	}

	public override void PrepAttackHandler()
	{
		if (currentState == State.PREP_PUNCH && Time.GetTicksMsec() - TimePrepAttackStart > TimePrepAttackDuration)
		{
			currentState = State.ATTACK;
			attack_animations = attack_animations.Shuffle().ToArray();
			TimeLastAttackStart = Time.GetTicksMsec();
			GD.Print(attack_animations[0]);
		}
	}


	public override void InputHandler()
	{
		if (player != null && CanMove())
		{
			if (enemySlot == null)
			{
				enemySlot = player.ReserveSlot(this);
			}

			if (enemySlot != null)
			{
				if (IsPlayerWithInRange())
				{
					Velocity = Vector2.Zero;
					if (CanPunch())
					{
						currentState = State.PREP_PUNCH;
						TimePrepAttackStart = Time.GetTicksMsec();
					}

				}
				else
				{
					var direciton = (enemySlot.GlobalPosition - Position).Normalized();
					Velocity = direciton * speed;
				}
			}

		}
	}

	private bool IsPlayerWithInRange()
	{
		return (enemySlot.GlobalPosition - Position).Length() < 1;
	}

	public override bool CanPunch()
	{
		if ((Time.GetTicksMsec() - TimeLastAttackStart) < TimeLastAttackDuration)
		{
			return false;
		}
		return base.CanPunch();
	}

	public override void OnDamageReceiver(int damage, Vector2 direction, int HitType)
	{
		base.OnDamageReceiver(damage, direction, HitType);
		if (current_health <= 0)
		{
			enemySlot.FreeSlotEnemy();
		}
	}


}
