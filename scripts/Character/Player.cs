using Godot;
using System;
using System.Reflection.Metadata;

public partial class Player : Character
{


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		stateMachine = GetNode<Node>("stateMachine");

		stateMachine.SetProcess(true);
		ChangeState(PreState.instance.idle,"idle");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public override void AnimationHandler()
	{
		// switch (currentState)
		// {
		// 	case State.idle:
		// 		animation.Play("idle");
				
		// 		break;
		// 	case State.walk:
		// 		animation.Play("walk");
				
		// 		break;
		// }
	}



	public override void MoveHandler()
	{
		// Vector2 direciton = Input.GetVector("left", "right", "up", "down");
		
		// if (direciton == Vector2.Zero)
		// {
		// 	ChangeState(idle,"idle");
			
		// }
		// else
		// {
		// 	ChangeState(walk,"walk");
		// }
		// Velocity = direciton * speed;

		// float horizontal = Input.GetAxis("left", "right");
		// if (horizontal > 0)
		// {
		// 	heading = false;
		// }
		// else if (horizontal < 0)
		// {
		// 	heading = true;
		// }

		// skin.FlipH = heading;

		// MoveAndSlide();
	}
}
