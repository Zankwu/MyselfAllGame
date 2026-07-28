using Godot;
using System;

public partial class Character : CharacterBody2D
{


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
	public Node stateMachine;

	public string currentState = "idle";
	public bool heading = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// stateMachine = GetNode<Node>("stateMachine");

	}

	public virtual void ChangeState(Script script, string name)
	{



		stateMachine = GetNode<Node>("stateMachine");
		// if (stateMachine == null || !GodotObject.IsInstanceValid(stateMachine))
		// {
		// 	GD.PrintErr("❌ stateMachine 无效！");
		// 	return;
		// }

		// if (script == null || !script.CanInstantiate())
		// {
		// 	GD.PrintErr("❌ 脚本无效！");
		// 	return;
		// }

		stateMachine.SetScript(script);


	}

	public override void _Process(double delta)
	{
		// MoveHandler();
		AnimationHandler();
		MoveAndSlide();
	}

	public virtual void AnimationHandler()
	{

	}

	public virtual void MoveHandler()
	{

	}
}
