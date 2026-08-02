using Godot;
using System;

public partial class BasicEnemy : Character
{


	[Export]
	public Player player;

	public EnemySlot enemySlot;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public override void HeadingHandler()
	{
		if (currentState == State.FALL)
		{
			GD.Print($"message: {height}");
		}
		float temp = GlobalPosition.X - player.GlobalPosition.X;
		if (temp > 0)
		{
			heading = true;
		}
		else
		{
			heading = false;
		}
		base.HeadingHandler();
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
				if ((enemySlot.GlobalPosition - Position).Length() < 1)
				{
					Velocity = Vector2.Zero;
				}
				else
				{
					var direciton = (enemySlot.GlobalPosition - Position).Normalized();
					Velocity = direciton * speed;
				}


			}

		}
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
