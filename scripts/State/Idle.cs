using Godot;
using System;

public partial class Idle : Node
{
    Character player = null;
    public override void _Ready()
    {
        GD.Print("✅ Idle 状态已加载！");
        player = GetParent() as Character;
        player.animation.Play("idle");
    }
    public override void _Process(double delta)
    {
        if (player == null)
        {
            player = GetParent() as Character;

        }
        GD.Print($"message: idle");
        base._Process(delta);
        Vector2 direciton = Input.GetVector("left", "right", "up", "down");
        if (direciton != Vector2.Zero)
        {
            player.ChangeState(PreState.instance.walk, "idle");
        }

    }



}
