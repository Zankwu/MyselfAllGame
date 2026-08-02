using Godot;
using System;

public partial class EnemySlot : Node2D
{
	//槽位下面有一个敌人
	public BasicEnemy enemy;

	//槽位是否有敌人
	public bool IsEnemyFree()
	{
		return enemy == null;
	}

	//设置槽位的敌人
	public void SetSlotEnemy(BasicEnemy enemy_temp)
	{
		enemy = enemy_temp;
	}

	//释放敌人
	public void FreeSlotEnemy()
	{
		enemy = null;
	}
}
