using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class CheckPoint : Node
{
	[Export]
	public Node2D enemies;
	[Export]
	public int alive_enemy_max;
	public int alive_enemies;
	public bool is_activited;

	public List<EnemyData> enemyDatas = new List<EnemyData>();
	public Array<EnemyData> enemyDatasTemp = new Array<EnemyData>();
	[Export]
	public Area2D playerDetectionArea;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerDetectionArea.BodyEntered += OnPlayerEntered;
		EntityManager.instance.OnEnemyDeath += OnEnemyDeath;
		foreach (Node2D child in enemies.GetChildren())
		{
			Character temp1 = child as Character;
			enemyDatasTemp.Add(new EnemyData(temp1.tYPE, temp1.GlobalPosition));
			temp1.QueueFree();
		}
	}

	private void OnEnemyDeath(BasicEnemy enemy)
	{
		alive_enemies -= 1;
	}

	private void OnPlayerEntered(Node2D body)
	{
		if (!is_activited)
		{
			is_activited = true;
			GD.Print(enemyDatasTemp.Count);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (is_activited && enemyDatasTemp.Count() > 0)
		{
			if (alive_enemies < alive_enemy_max)
			{
				EnemyData enemyData = enemyDatasTemp[0];
				EntityManager.instance.EmitSignal(EntityManager.SignalName.OnSpawnEnemy, enemyData);
				alive_enemies += 1;
				enemyDatasTemp.RemoveAt(0);
			}
		}
	}
}
