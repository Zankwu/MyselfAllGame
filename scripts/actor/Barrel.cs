using Godot;

public partial class Barrel : StaticBody2D
{
	public enum State
	{
		idle, destoryed
	}

	[Export]
	public Sprite2D skin;

	[Export]
	public DamageReceiver damageReceiver;

	Vector2 velocity = new Vector2();
	public float KNOCK_FORCE = 50;

	public float height = 0;

	public float height_speed = 0;

	public float Gravity = 600;

	public State currentState = State.idle;

	[Export]
	public Collectible.TYPE insideType = Collectible.TYPE.FOOD;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		damageReceiver.DamageReceived += OnDamageReceiver;
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += velocity * (float)delta;
		skin.Position = Vector2.Up * height;
		AirTimeHandler((float)delta);
	}

	private void OnDamageReceiver(int damage, Vector2 direction, int HitType)
	{
		if (currentState == State.idle)
		{
			currentState = State.destoryed;
			velocity = direction * KNOCK_FORCE;
			height_speed = KNOCK_FORCE * 2;
			EntityManager.instance.EmitSignal(EntityManager.SignalName.OnCollectibleSpawn,
				(int)insideType, (int)Collectible.State.FALL,
				GlobalPosition, Vector2.Zero, 0,false
			);
		}

	}

	public void AirTimeHandler(float delta)
	{
		if (currentState == State.destoryed)
		{
			skin.Frame = 1;
			Color temp = skin.Modulate;
			temp.A -= delta;
			skin.Modulate = temp;
			height += height_speed * delta;
			if (height < 0)
			{
				height = 0;
				QueueFree();
			}

		}
		height_speed -= Gravity * delta;

	}
}
