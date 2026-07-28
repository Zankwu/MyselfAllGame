using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Character : CharacterBody2D
{
	[Export]
	public Label labelState;

	[Export]
	public int health;
	[Export]

	public int damage;
	[Export]
	public int speed;

	[Export]
	public AnimationPlayer animation;

	[Export]
	public Sprite2D skin;

	[Export]
	public StateMachine stateMachine;

	public string currentState = "idle";
	public bool heading = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// stateMachine = GetNode<Node>("stateMachine");

	}

	public virtual void ChangeState(Script script, string name)
	{
		stateMachine = GetNode<StateMachine>("stateMachine");
		if (stateMachine.HasMethod("ChangeStateEnd"))
		{
			stateMachine.ChangeStateEnd();
		}
		stateMachine.SetScript(script);
		stateMachine.ChangeStateBegin();
		currentState = name;
		labelState.Text = currentState;
	}

	public override void _Process(double delta)
	{
		// MoveHandler();
		AnimationHandler();
		HeadingHandler();
		MoveAndSlide();
	}

	private void HeadingHandler()
	{
		float horizontal = Input.GetAxis("left", "right");
		if (horizontal > 0)
		{
			heading = false;
		}
		else if (horizontal < 0)
		{
			heading = true;
		}

		skin.FlipH = heading;
	}

	public virtual void AnimationHandler()
	{
		animation.Play(currentState);

	}

	public virtual void MoveHandler()
	{

	}
}
