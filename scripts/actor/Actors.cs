using Godot;
using System;
using System.Collections.Generic;

public partial class Actors : Node2D
{
	PackedScene shotPacked = GD.Load<PackedScene>("res://scenes/Actors/shot.tscn");
	public Dictionary<Collectible.TYPE, PackedScene> collectibleMaps = new Dictionary<Collectible.TYPE, PackedScene>
	{
		{Collectible.TYPE.KNIFE,GD.Load<PackedScene>("res://scenes/Collectibles/knife.tscn")},

	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EntityManager.instance.OnCollectibleSpawn += OnCollectibleSpawn;
		EntityManager.instance.SpawnShot += OnSpawnShot;
	}

    private void OnSpawnShot(Vector2 gun_root_position, float distance, float height)
    {
        Shot shot = shotPacked.Instantiate<Shot>();
		AddChild(shot);
		shot.initialize(distance,height);
		shot.Position = gun_root_position;
		
    }

    private void OnCollectibleSpawn(int tYPE, int state, Vector2 positon, Vector2 heading,float initialHeight)
	{
		PackedScene packedScene = collectibleMaps[(Collectible.TYPE)tYPE];
		Collectible collectible = packedScene.Instantiate<Collectible>();
		collectible.GlobalPosition = positon;
		collectible.currentState = (Collectible.State)state;
		collectible.heading = heading;
		collectible.height += initialHeight;
		AddChild(collectible);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
