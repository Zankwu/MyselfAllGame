using Godot;
using System;

public partial class Idle : StateMachine
{


    public override void _Ready()
    {


    }

    public override void ChangeStateBegin()
    {
        GD.Print("✅ Idle 状态已加载！");

    }

    public override void ChangeStateEnd()
    {
        GD.Print("❌ idle 状态已结束！");
    }
    public override void _Process(double delta)
    {


    }



}
