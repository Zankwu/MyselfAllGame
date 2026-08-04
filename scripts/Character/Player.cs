using Godot;
using System;
using System.Linq;
using System.Reflection.Metadata;

public partial class Player : Character
{
	[Export]
	public EnemySlot[] enemySlots;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		attack_animations = ["PUNCH", "PUNCH_AIT", "KICK", "ROUND_KICK"];

		base._Ready();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void AnimationHandler()
	{
		base.AnimationHandler();
	}


	public override void HeadingHandler()
	{
		float horizontal = Input.GetAxis("left", "right");

		if (horizontal > 0)
		{
			heading = Vector2.Right;

		}
		else if (horizontal < 0)
		{
			heading = Vector2.Left;
		}
		base.HeadingHandler();

	}


	public override void InputHandler()
	{
		Vector2 direciton = Input.GetVector("left", "right", "up", "down");
		Velocity = direciton * speed;

		if (Input.IsActionJustPressed("attack") && CanPunch())
		{
			if (is_last_attack_sucessful)
			{
				is_last_attack_sucessful = false;
				attack_combo_index = (attack_combo_index + 1) % attack_animations.Length;

			}
			currentState = State.ATTACK;
		}
		if (Input.IsActionJustPressed("jump") && CanJump())
		{
			currentState = State.TAKEOFF;
			height_speed = JUMPFORCE;
		}
		if (Input.IsActionJustPressed("attack") && CanJumpKick())
		{
			currentState = State.JUMPKICK;
		}

	}

	public EnemySlot ReserveSlot(BasicEnemy enemy)
	{
		//获得空闲槽位
		EnemySlot[] availableSlots = enemySlots.Where(n => n.IsEnemyFree()).ToArray();
		if (availableSlots.Length == 0)
		{
			return null;
		}
		//空闲槽位排序出最近的槽位
		availableSlots.Sort((a, b) =>
		{
			float dist_a = (a.GlobalPosition - enemy.GlobalPosition).Length();
			float dist_b = (b.GlobalPosition - enemy.GlobalPosition).Length();
			return dist_a.CompareTo(dist_b);
		});
		availableSlots[0].enemy = enemy;
		return availableSlots[0];


	}

	public void FreeEnemySlot(BasicEnemy enemy)
	{
		EnemySlot[] targetSlots = enemySlots.Where(n => n.enemy == enemy).ToArray();
		if (targetSlots.Length == 1)
		{
			targetSlots[0].FreeSlotEnemy();
		}
	}
}
