using Godot;
using System;
using System.Collections.Generic;

public partial class Ui : Node
{
    public Dictionary<Character.CharacterType, Texture2D> avatersMaps = new Dictionary<Character.CharacterType, Texture2D>
    {
        {Character.CharacterType.Basic,GD.Load<Texture2D>("res://accetss/art/ui/avatars/avatar-punk.png")},
        {Character.CharacterType.Goon,GD.Load<Texture2D>("res://accetss/art/ui/avatars/avatar-goon.png")},
        {Character.CharacterType.Thug,GD.Load<Texture2D>("res://accetss/art/ui/avatars/avatar-thug.png")},

    };
    [Export]
    public HelathBar playerBar;
    [Export]
    public HelathBar enemyBar;
    [Export]
    public TextureRect enemyAvater;

    public float healthBar_visible_start = Time.GetTicksMsec();
    public float healthBar_visible_duration = 1000;
    public override void _Ready()
    {
        DamageManager.instance.HealthBarChange += HealthChange;
        enemyAvater.Visible = false;
        enemyBar.Visible = false;
    }

    private void HealthChange(int type, int max_health, int current_health)
    {
        if ((Character.CharacterType)type == Character.CharacterType.Player)
        {
            playerBar.Refresh(max_health, current_health);

        }
        else
        {
            healthBar_visible_start = Time.GetTicksMsec();
            enemyBar.Refresh(max_health, current_health);
            enemyAvater.Texture = avatersMaps[(Character.CharacterType)type];
            enemyAvater.Visible = true;
            enemyBar.Visible = true;
        }

    }


    public override void _Process(double delta)
    {
        if (enemyBar.Visible && Time.GetTicksMsec() - healthBar_visible_start > healthBar_visible_duration)
        {
            enemyAvater.Visible = false;
            enemyBar.Visible = false;
        }
    }
}
