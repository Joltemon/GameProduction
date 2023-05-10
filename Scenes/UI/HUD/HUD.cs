using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Export] Player? Player;
	[Export] Control? Root;
	[Export] Label? AmmoLabel;
	[Export] public Label? TimerLabel;
	[Export] ProgressBar? SprintBar;
	[Export] GpuParticles2D? SprintingParticle;
	[Export] AnimationPlayer? Animation;

	[Export] float SpeedLimit;

	void UpdateAmmunition(int ammo)
	{
		if (AmmoLabel != null)
			AmmoLabel.Text = ammo.ToString();
	}

	public void UpdateSpeedEffects(float currentSpeed) 
	{
		if (SprintingParticle == null) return;

		if (currentSpeed > SpeedLimit) 
		{
			SprintingParticle.Emitting = true;
		}
		else if (currentSpeed < SpeedLimit)
		{
			SprintingParticle.Emitting = false;
		}
	}

	public void UpdateTimer(double seconds)
	{
		if (TimerLabel == null) return;

		var time = TimeSpan.FromSeconds(seconds);

		TimerLabel.Text = $"{((int)time.TotalMinutes).ToString("0")}:{time.Seconds.ToString("00")}:{time.Milliseconds.ToString("00")}";
	}

	public override void _Ready()
	{
		Animation?.Play("Show");
	}

	public override void _Process(double delta) {
		if (Root != null)
			Root.Position = Root.Position.Lerp(-Input.GetLastMouseVelocity()/128, 0.5f);

		if (Player == null) return;

		if (SprintBar != null) SprintBar.Value = Mathf.Lerp(SprintBar.Value, Player.SprintEnergy, 8 * (float)delta);
		UpdateTimer(Player.Stopwatch);
	}

	void Finished() {
		TimerLabel?.Set("theme_override_colors/font_color", Color.Color8(0, 232, 0));
	}
}
