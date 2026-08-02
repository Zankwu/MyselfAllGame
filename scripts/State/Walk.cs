using Godot;
using System;

public partial class Walk : StateMachine
{



    public override void _Ready()
    {

    }

    public override void ChangeStateBegin()
    {
        GD.Print("✅ walk 状态已加载！");


    }
    public override void ChangeStateEnd()
    {
        GD.Print("❌ walk 状态已结束！");

    }

    public override void _Process(double delta)
    {
    //     if (player == null)
    //     {
    //         player = GetParent() as Character;

    //     }

    //     Vector2 direciton = Input.GetVector("left", "right", "up", "down");

    //     if (direciton == Vector2.Zero)
    //     {
    //         if (player.currentState != "idle")
    //         {
    //             player.ChangeState(PreState.instance.idle, "idle");

    //         }

    //     }
    //     if (Input.IsActionJustPressed("attack") && base.CanPunch(player))
    //     {

    //         player.ChangeState(PreState.instance.attack, "punch");
    //     }
    //     player.Velocity = direciton * player.speed;
    }
}
