using Godot;
using System;

public partial class ScoreIndicator : Label
{
	public int max_score;
	public int current_score;
	public bool is_convert = false;
	public float score_duration = 1000;
	public float score_start = Time.GetTicksMsec();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ComboNumber comboNumber = GetNode<ComboNumber>("../ComboNumber");
		comboNumber.ScoreChange += Convert;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (is_convert)
		{
			float progress = (Time.GetTicksMsec() - score_start) / score_duration;
			progress = Mathf.Clamp(progress, 0.0f, 1.0f);
			current_score = (int)Mathf.Lerp(current_score, max_score, progress);

			if (progress >= 1)
			{
				current_score = max_score;
				is_convert = false;
			}
			Refresh();

		}

	}

	public void Convert(int ComboNumber)
	{
		for (int i = 1; i <= ComboNumber; i++)
		{
			max_score += i;
		}
		score_start = Time.GetTicksMsec();
		is_convert = true;
		Refresh();
	}

	private void Refresh()
	{

		Text = $"{current_score}";
	}

}
