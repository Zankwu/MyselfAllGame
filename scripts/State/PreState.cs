using Godot;
using System;

public partial class PreState : StateMachine
{
    public Script idle = GD.Load<Script>("res://scripts/State/Idle.cs");
    public Script walk = GD.Load<Script>("res://scripts/State/Walk.cs");

    public Script attack = GD.Load<Script>("res://scripts/State/Attack.cs");

    public static PreState instance { get; set; }

    public override void _Ready()
    {


        instance = this;
        base._Ready();
    }

}
