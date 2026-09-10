using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public partial class Actors : Node2D
{
	[Export]
	public Player player;
	PackedScene shotPacked = GD.Load<PackedScene>("res://scenes/Actors/shot.tscn");

	PackedScene sparkPacked = GD.Load<PackedScene>("res://scenes/Actors/spark.tscn");

	public Dictionary<Collectible.TYPE, PackedScene> collectibleMaps = new Dictionary<Collectible.TYPE, PackedScene>
	{
		{Collectible.TYPE.KNIFE,GD.Load<PackedScene>("res://scenes/Collectibles/knife.tscn")},
		{Collectible.TYPE.GUN,GD.Load<PackedScene>("res://scenes/Collectibles/gun.tscn")},
		{Collectible.TYPE.FOOD,GD.Load<PackedScene>("res://scenes/Collectibles/food.tscn")},

	};
	public Dictionary<Character.CharacterType, PackedScene> characterMaps = new Dictionary<Character.CharacterType, PackedScene>
	{
		{Character.CharacterType.Basic,GD.Load<PackedScene>("res://scenes/Character/basic_enemy.tscn")},
		{Character.CharacterType.Goon,GD.Load<PackedScene>("res://scenes/Character/goon_enemy.tscn")},
		{Character.CharacterType.Thug,GD.Load<PackedScene>("res://scenes/Character/thug_enemy.tscn")},
		{Character.CharacterType.Boss,GD.Load<PackedScene>("res://scenes/Character/igro_boss.tscn")},
	};
	// Called when the node enters the scene tree for the first time.
	
	public List<Door> doorList = [];
	public override void _Ready()
	{
		EntityManager.instance.OnCollectibleSpawn += OnCollectibleSpawn;
		EntityManager.instance.SpawnShot += OnSpawnShot;
		EntityManager.instance.OnSpawnEnemy += OnSpawnEnemy;
		EntityManager.instance.OrphanActor += OrphanActorReparents;
		EntityManager.instance.SpawnSpark += OnSpawnSpark;
		player = GetNode<Player>("Player");
	}

    private void OnSpawnSpark(Vector2 position)
    {
        Node2D spark=sparkPacked.Instantiate<Node2D>();
		spark.Position = position;
		AddChild(spark);
    }

    private void OrphanActorReparents(Node2D orphan)
	{
		GD.Print("receive");
		if (orphan is Door door)
		{
			doorList.Add(door);
		}
		orphan.Reparent(this);
	}

	private void OnSpawnShot(Vector2 gun_root_position, float distance, float height)
	{
		Shot shot = shotPacked.Instantiate<Shot>();
		AddChild(shot);
		shot.initialize(distance, height);
		shot.Position = gun_root_position;

	}

	private void OnCollectibleSpawn(int tYPE, int state, Vector2 positon,
	 Vector2 heading, float initialHeight, bool dorpCanDestoryed)
	{
		PackedScene packedScene = collectibleMaps[(Collectible.TYPE)tYPE];
		Collectible collectible = packedScene.Instantiate<Collectible>();
		collectible.GlobalPosition = positon;
		collectible.currentState = (Collectible.State)state;
		collectible.heading = heading;
		collectible.height += initialHeight;
		collectible.onDropCanDestoryed = dorpCanDestoryed;
		CallDeferred(Node.MethodName.AddChild, collectible);
	}
	private void OnSpawnEnemy(EnemyData enemyData)
	{
		PackedScene scene = characterMaps[enemyData.character_type];
		BasicEnemy tempChar = scene.Instantiate() as BasicEnemy;
		tempChar.tYPE = enemyData.character_type;
		tempChar.GlobalPosition = enemyData.global_position;
		tempChar.player = player;
		tempChar.height = enemyData.height;
		tempChar.currentState = enemyData.currentState;
		if (enemyData.Door_Index > -1)
		{
			tempChar.Assign_Door(doorList[enemyData.Door_Index]);
			tempChar.Modulate = tempChar.Modulate with{A=0f};
		}
		AddChild(tempChar);

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
