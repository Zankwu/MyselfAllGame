using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class SfxManager : Node
{
    public static SfxManager instance;

    [Export]
    public Array<AudioStreamPlayer> sfx_list = new();

    public enum SFX_TYPE
    {
        CLICK,FOOD,GO,GRUNT,GUNSHOT,HIT1,HIT2,KNIFE_HIT,MISS
    }

    public override void _Ready()
    {
        instance = this;
    }


    public void PlaySFX(SFX_TYPE tYPE)
    {
        sfx_list[(int)tYPE].Play();
    }
}
