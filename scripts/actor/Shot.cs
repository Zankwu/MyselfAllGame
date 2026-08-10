using System;
using Godot;


public partial class Shot : Line2D
{
	//子弹穿过屏幕所需时间的最大值
	[Export]
	public ulong shot_across_window_duration;

	//射击距离所需的时间
	public float duration_shot;
	//设计高度
	public float height;
	//射击距离
	public float shot_distance;

	public ulong time_start = Time.GetTicksMsec();
	public void initialize(float distance, float gun_height)
	{
		height = gun_height;
		shot_distance = distance;
		AddPoint(new Vector2(0, -height), 0);
		AddPoint(new Vector2(distance, -height), 1);
		duration_shot = Mathf.Abs(shot_distance) * shot_across_window_duration / GetViewportRect().Size.X;

	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		var lerped = Time.GetTicksMsec() - time_start;
		float progress = lerped / duration_shot;
		var newX = Mathf.Lerp(0, shot_distance, progress);
		SetPointPosition(0, new Vector2(newX, -height));
		if (progress > 1)
		{
			QueueFree();
		}
	}
}



/* 		height = gun_height;
		shot_distance = distance;
		AddPoint(new Vector2(0,-height),0);
		AddPoint(new Vector2(distance,-height),1);
		duration_shot = Mathf.Abs(shot_distance) * shot_across_window_duration / GetViewportRect().Size.X;
	*/
