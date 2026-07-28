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
        if (player == null)
        {
            player = GetParent() as Character;
        }
        Vector2 direciton = Input.GetVector("left", "right", "up", "down");

        if (direciton != Vector2.Zero)
        {
            if (player.currentState != "walk" && player.currentState != "attack")
            {
                player.ChangeState(PreState.instance.walk, "walk");

            }
        }
        if(Input.IsActionJustPressed("attack")){
            player.ChangeState(PreState.instance.attack, "punch");
            
        }
    

    }



}
