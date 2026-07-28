using Godot;
using System;

public partial class StateMachine : Node
{
	public Character player = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public virtual void ChangeStateBegin()
	{

	}

	public virtual void ChangeStateEnd()
	{

	}

	public virtual bool CanPunch(Character player)
	{
		return player.currentState == "idle" || player.currentState == "walk";
	}

	public virtual void OnActionComplete()
	{
		player.ChangeState(PreState.instance.idle, "idle");

	}
}
