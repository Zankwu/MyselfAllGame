using Godot;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public partial class BasicEnemy : Character
{
	[Export]
	public RayCast2D rayCast2D;

	public float EDGE_SCRREN_BUFFER = 8;


	public EnemySlot enemySlot;
	[Export]
	public Player player;

	[Export]
	public ulong TimeLastAttackDuration_Melee = 2000;
	[Export]
	public ulong TimeLastAttackDuration_Range = 2000;


	[Export]
	public ulong TimePrepAttackDuration = 500;

	public ulong TimePrepAttackStart = Time.GetTicksMsec();
	public ulong TimeLastAttackStart_Melee = Time.GetTicksMsec();
	public ulong TimeLastAttackStart_Range = Time.GetTicksMsec();


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
		rayCast2D.Scale = heading == Vector2.Right ? new Vector2(1,1) : new Vector2(-1,-1);
		base.HeadingHandler();
	}

	public override void PrepAttackHandler()
	{
		if (currentState == State.PREP_PUNCH && Time.GetTicksMsec() - TimePrepAttackStart > TimePrepAttackDuration)
		{
			currentState = State.ATTACK;
			attack_animations = attack_animations.Shuffle().ToArray();
			TimeLastAttackStart_Melee = Time.GetTicksMsec();
			GD.Print(attack_animations[0]);
		}
	}


	public override void InputHandler()
	{
		if (player != null && CanMove())
		{
			if (can_respawn_knife)
			{
				AttackWithRange();
			}
			else
			{
				AttackWithMelee();
			}
		}
	}
	public void AttackWithRange()
	{
		var camera = GetViewport().GetCamera2D();
		var screenWidth = GetViewportRect().Size.X;

		var screenLeft = camera.Position.X - screenWidth / 2;
		var screenRight = camera.Position.X + screenWidth / 2;

		Vector2 clostPosition = Vector2.Zero;
		Vector2 targetLeft = new Vector2(screenLeft + EDGE_SCRREN_BUFFER / 2, player.Position.Y);
		Vector2 targetRight = new Vector2(screenRight - EDGE_SCRREN_BUFFER, player.Position.Y);

		if ((targetLeft - Position).Length() < (targetRight - Position).Length())
		{
			clostPosition = targetLeft;
		}
		else
		{
			clostPosition = targetRight;
		}
		if ((clostPosition - Position).Length() < 1)
		{
			Velocity = Vector2.Zero;
		}
		else
		{
			Velocity = (clostPosition - Position).Normalized() * speed;
		}

		if (CanThrow() && hasKnfie && rayCast2D.IsColliding())
		{
			currentState = State.THROW;
			Time_Knife_dismiss = Time.GetTicksMsec();
			TimeLastAttackStart_Range = Time.GetTicksMsec();
		}
	}

	public void AttackWithMelee()
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

	private bool IsPlayerWithInRange()
	{
		return (enemySlot.GlobalPosition - Position).Length() < 1;
	}

	public override bool CanPunch()
	{
		if ((Time.GetTicksMsec() - TimeLastAttackStart_Melee) < TimeLastAttackDuration_Melee)
		{
			return false;
		}
		return base.CanPunch();
	}

	public bool CanThrow()
	{
		if (Time.GetTicksMsec() - TimeLastAttackStart_Range < TimeLastAttackDuration_Range)
		{
			return false;
		}
		return true;
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
