using Godot;
using System;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Security.Principal;

public partial class Player : Character
{
	[Export]
	public EnemySlot[] enemySlots;

	[Export]
	public ulong Time_Combo_Duration;

	public ulong Time_Combo_Last = Time.GetTicksMsec();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		attack_animations = ["PUNCH", "PUNCH_AIT", "KICK", "ROUND_KICK"];

		base._Ready();


	}

	private void OnCollectibleEntered(Node2D body)
	{
		if (Input.IsActionJustPressed("attack") && !hasKnfie)
		{
			currentState = State.PICKUP;
			hasKnfie = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		base._Process(delta);
		AttackComboHandler();
	}

    private void AttackComboHandler()
	{
		if (Time.GetTicksMsec() - Time_Combo_Last > Time_Combo_Duration)
		{
			attack_combo_index = 0;
		}
	}

    public override void AnimationHandler()
	{
		base.AnimationHandler();
	}


	public override void HeadingHandler()
	{
		float horizontal = Input.GetAxis("left", "right");

		if (currentState == State.GROUNDED)
		{
			return;
		}
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
			if (hasKnfie)
			{
				currentState = State.THROW;
				Time_Knife_dismiss = Time.GetTicksMsec();
			}
			else if (hasGun)
			{
				if (arrmoLeft <= 0)
				{
					currentState = State.THROW;
				}
				else
				{
					GunShoot();
					arrmoLeft -= 1;
				}
			}
			else if (CanPickUpCollectible())
			{
				currentState = State.PICKUP;
			}
			else if (is_last_attack_sucessful)
			{
				Time_Combo_Last = Time.GetTicksMsec();
				is_last_attack_sucessful = false;
				attack_combo_index = (attack_combo_index + 1)
				% attack_animations.Length;
				currentState = State.ATTACK;
			}
			else
			{
				currentState = State.ATTACK;
			}
		}
		if (Input.IsActionJustPressed("jump") && CanJump())
		{
			currentState = State.TAKEOFF;
			height_speed = JUMPFORCE;
			attack_combo_index = 0;
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
