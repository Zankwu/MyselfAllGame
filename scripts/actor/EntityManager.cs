using Godot;
using System;

public partial class EntityManager : Node
{
    [Signal]
    public delegate void OnCollectibleSpawnEventHandler(int tYPE, int state,
     Vector2 positon, Vector2 heading,float height,bool dorpCanDestoryed);

    [Signal]
    public delegate void SpawnShotEventHandler(Vector2 gun_root_position,float distance,float height);
    public static EntityManager instance;

    public override void _Ready()
    {
        instance = this;
    }


}
