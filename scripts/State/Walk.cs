using Godot;
using System;

public partial class Walk : Node
{

    Character player = null;

    public override void _Ready()
    {
        GD.Print("✅ Walk 状态已加载！");
        player = GetParent() as Character;
        player.animation.Play("walk");
    }
    public override void _Process(double delta)
    {
        if (player == null)
        {
            player = GetParent() as Character;

        }
        GD.Print($"message: walk");
        base._Process(delta);
        //animation.Play("idle");
        Vector2 direciton = Input.GetVector("left", "right", "up", "down");
        if (direciton == Vector2.Zero)
        {
            player.ChangeState(PreState.instance.idle, "idle");
        }
        player.Velocity = direciton * player.speed;
    }



}
