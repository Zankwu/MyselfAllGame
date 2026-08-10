using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Character : CharacterBody2D
{
	public enum State
	{
		IDLE, WALK, ATTACK, TAKEOFF, JUMP, LAND, JUMPKICK,
		HURT, FALL, GROUNDED, DEATH, FLY, PREP_PUNCH, THROW, PICKUP,
		SHOOT
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
		{State.THROW,"THROW"},
		{State.PICKUP,"PICKUP"},
		{State.SHOOT,"SHOOT"},
	};
	[Export]
	public AnimationPlayer animation;
	public string[] attack_animations;
	public int attack_combo_index = 0;
	[Export]
	public bool can_respawn;
	[Export]
	public bool can_respawn_knife = false;
	[Export]
	public DamageEmitter chainReactionEmit;
	[Export]
	public CollisionShape2D collisionShape2D;
	public int current_health;
	public State currentState = State.IDLE;

	[Export]
	public Area2D collectibleSensor;
	[Export]
	public int damage;
	[Export]
	public DamageEmitter damageEmitter;
	[Export]
	public DamageReceiver damageReceiver;
	[Export]
	public float FLY_FORCE = 100;
	public float GRAVITY = 600f;
	public Vector2 heading = Vector2.Right;
	public float height = 0f;
	public float height_speed = 0f;
	[Export]
	public bool hasKnfie = false;
	[Export]
	public bool hasGun = false;
	// 1 = NORMAL 2 = JUMPKICK 3 = POWER
	public int HitType;
	[Export]
	public Node2D weaponPositon;
	public bool is_last_attack_sucessful;
	public float JUMPFORCE = 150f;
	[Export]
	public Sprite2D knifeSprite;

	[Export]
	public Sprite2D gunSprite;
	[Export]
	public float KNOCK_BACK_FORCE = 50;
	[Export]
	public float KNOCK_DOWN_FORCE = 70;
	[Export]
	public Label labelState;
	[Export]
	public int max_health;

	[Export]
	public RayCast2D rayCast2D;
	[Export]
	public Sprite2D skin;
	[Export]
	public int speed;
	[Export]
	public StateMachine stateMachine;
	public ulong Time_Death_Start = Time.GetTicksMsec();
	[Export]
	public ulong Time_Grounded_Duration = 1000;
	public ulong Time_Grounded_Start = Time.GetTicksMsec();
	[Export]
	public ulong Time_Knife_Respawn_duration = 2000;
	public ulong Time_Knife_dismiss = Time.GetTicksMsec();


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
		KnifeRespawnHandler();
		MoveAndSlide();


		if (labelState != null)
		{
			labelState.Text = animationMap[currentState];

		}
		damageEmitter.Monitoring = currentState == State.ATTACK || currentState == State.JUMPKICK;
		collisionShape2D.Disabled = IsCollisionShape2DEnable();
		skin.Position = Vector2.Up * height;
		knifeSprite.Position = Vector2.Up * height;
		knifeSprite.Visible = hasKnfie;
		gunSprite.Visible = hasGun;
		gunSprite.Position = Vector2.Up * height;
		chainReactionEmit.Monitoring = currentState == State.FLY;
		damageReceiver.Monitorable = CanGetHurt();

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
			knifeSprite.Scale = new Vector2(1, knifeSprite.Scale.Y);
			gunSprite.Scale = new Vector2(1, knifeSprite.Scale.Y);
		}
		else if (heading == Vector2.Left)
		{
			damageEmitter.Scale = new Vector2(-1, damageEmitter.Scale.Y);
			skin.FlipH = true;
			knifeSprite.Scale = new Vector2(-1, knifeSprite.Scale.Y);
			gunSprite.Scale = new Vector2(-1, knifeSprite.Scale.Y);
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
	public virtual void KnifeRespawnHandler()
	{
		if (can_respawn_knife && !hasKnfie &&
		(Time.GetTicksMsec() - Time_Knife_dismiss > Time_Knife_Respawn_duration))
		{
			hasKnfie = true;
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
		|| currentState == State.PREP_PUNCH
		;
	}
	public bool CanPickUpCollectible()
	{
		Area2D[] areas = collectibleSensor.GetOverlappingAreas().ToArray();
		if (areas.Length < 1)
		{
			return false;
		}
		Collectible tempColl = areas[0] as Collectible;
		if (tempColl.currentType == Collectible.TYPE.KNIFE && !hasKnfie)
		{
			return true;
		}

		if (tempColl.currentType == Collectible.TYPE.GUN && !hasGun)
		{
			return true;
		}
		return false;
	}

	public void PickUpCollectible()
	{
		Area2D[] areas = collectibleSensor.GetOverlappingAreas().ToArray();
		Collectible tempColl = areas[0] as Collectible;

		if (tempColl.currentType == Collectible.TYPE.KNIFE)
		{
			hasKnfie = true;
		}
		if (tempColl.currentType == Collectible.TYPE.GUN)
		{
			hasGun = true;
		}
		tempColl.QueueFree();
	}
	public void GunShoot()
	{
		currentState = State.SHOOT;
		Velocity = Vector2.Zero;
		Vector2 targetPoint = heading * (GlobalPosition.X + GetViewportRect().Size.X);
		var target = rayCast2D.GetCollider();
		if (target != null)
		{
			targetPoint = rayCast2D.GetCollisionPoint();
			// 对命中目标的 DamageReceiver 发射伤害信号（不要对自己的 damageReceiver 发射，否则会打到自己）
			DamageReceiver targetReceiver = (target as DamageReceiver)
				?? (target as Node)?.GetNodeOrNull<DamageReceiver>("DamageReceiver");
			targetReceiver?.EmitSignal(DamageReceiver.SignalName.DamageReceived, 8, heading, 2);
		}
		Vector2 gunPosition = new Vector2(weaponPositon.GlobalPosition.X, Position.Y);
		float gun_height = -weaponPositon.Position.Y;
		var distance = targetPoint.X - weaponPositon.GlobalPosition.X;
		EntityManager.instance.EmitSignal(EntityManager.SignalName.SpawnShot, gunPosition, distance,gun_height);
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
	public void OnThrowComplete()
	{
		currentState = State.IDLE;
		hasKnfie = false;
		Vector2 knifeGlobalPosition = new(weaponPositon.GlobalPosition.X, GlobalPosition.Y);
		float knife_height = -weaponPositon.Position.Y;
		EntityManager.instance.EmitSignal(EntityManager.SignalName.OnCollectibleSpawn, (int)Collectible.TYPE.KNIFE,
				(int)Collectible.State.FLY, knifeGlobalPosition, heading, knife_height);
	}
	public void OnPickUpComplete()
	{
		currentState = State.IDLE;
		PickUpCollectible();
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
			hasKnfie = false;
			can_respawn_knife = false;
			Time_Knife_dismiss = Time.GetTicksMsec();
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
