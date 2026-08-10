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
		
		//枪械高度
		height = gun_height;
		shot_distance = distance;
		//起点 Line会将起点和终点练成线
		AddPoint(new Vector2(0, -height), 0);
		//终点
		AddPoint(new Vector2(shot_distance, -height), 1);
		//射击所需时间 = 距离 * 子弹穿过时间最大值 / 屏幕距离
		duration_shot = Mathf.Abs(shot_distance) * shot_across_window_duration / GetViewportRect().Size.X;
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
		var lerped = Time.GetTicksMsec() - time_start;
		var progress = lerped / duration_shot;
		var newX = Mathf.Lerp(0, shot_distance, progress);
		SetPointPosition(0, new Vector2(newX, -height));
		if (progress >= 1)
		{
			QueueFree();
		}
	}
}



/* 		
			var lerped = Time.GetTicksMsec() - time_start;
		float progress = lerped / duration_shot;
		var newX = Mathf.Lerp(0, shot_distance, progress);
		SetPointPosition(0, new Vector2(newX, -height));
		if (progress > 1)
		{
			QueueFree();
		}
	*/
