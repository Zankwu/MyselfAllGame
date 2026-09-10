using Godot;
using System;

public partial class ComboNumber : Label
{

	[Signal]
	public delegate void ScoreChangeEventHandler(int x);

	public int combo_number = 0;

	public float time_combo_duration = 3000;
	public float time_combo_start = Time.GetTicksMsec();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DamageManager.instance.ComboChange += OnComboChange; 
		Refresh();
	}

	private void OnComboChange()
	{
		combo_number += 1;
		time_combo_start = Time.GetTicksMsec();
		Refresh();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (combo_number > 0 && Time.GetTicksMsec() - time_combo_start > time_combo_duration)
		{
			EmitSignal(SignalName.ScoreChange,combo_number);
			combo_number = 0;
			Refresh();
		}
	}


	public void Refresh()
	{
		Text = $"x{combo_number}";
		Visible = combo_number > 0;
	}
}
