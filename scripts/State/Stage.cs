using Godot;
using System;

public partial class Stage : Node2D
{
	[Export]
	public Node2D checkpoints;
	[Export]
	public Node2D containers;

	[Export]
	public Node2D Doors;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CallDeferred(nameof(EmitContainers));
	}
	public void EmitContainers()
	{
		GD.Print("emit");
		foreach (Node2D temp in containers.GetChildren())
		{
			EntityManager.instance.EmitSignal(EntityManager.SignalName.OrphanActor, temp);
			
		}

		for (int i = 0; i < Doors.GetChildCount(); i++)
		{
			Door door = Doors.GetChild(i) as Door;
			foreach (BasicEnemy enemy in door.basicEnemies)
			{
				enemy.assigin_door_index = i;
			}
		}

		foreach (Door door1 in Doors.GetChildren())
		{
			EntityManager.instance.EmitSignal(EntityManager.SignalName.OrphanActor, door1);

		}

		foreach (CheckPoint checkPoint in checkpoints.GetChildren())
		{
			checkPoint.CreateEnemyData();
		}
	}

}
