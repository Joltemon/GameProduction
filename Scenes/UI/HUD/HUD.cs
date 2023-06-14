using Godot;
using System;
using System.Threading.Tasks;

public partial class HUD : CanvasLayer
{
	[Export] Player? Player;
	[Export] Control? Root;
	[Export] Label? ScoreLabel;
	[Export] public Label? TimerLabel;
	[Export] ProgressBar? SprintBar;
	[Export] public GpuParticles2D? SprintingParticle;
	[Export] AnimationPlayer? Animation;
	[Export] Control? PauseMenu;
	[Export] IntermissionScreen? IntermissionScreen;

	[Export] TextureProgressBar? RestartProgressBar;
	Boolean ProgressBarPressed;
	double ProgressBarProgress = 0;

	[Export] public TextureProgressBar? ExtraSpeedBar;

	[Export] ColorRect? PixelationLayer;

	[Export] float SpeedLimit;

	[Signal] public delegate void CheckpointEventHandler();

	bool IsRestartPressed;

	public void UpdateScore(int score)
	{
		if (ScoreLabel != null)
			ScoreLabel.Text = score.ToString();
	}

	public void UpdateSpeedEffects(float currentSpeed, float dotValue) 
	{
		if (SprintingParticle == null) return;

		if (currentSpeed > SpeedLimit) 
		{
			SprintingParticle.Emitting = true;
		}
		else if (currentSpeed < SpeedLimit || dotValue <= 0)
		{
			SprintingParticle.Emitting = false;
		}
		
		// SprintingParticle.Amount = Mathf.RoundToInt(currentSpeed) + 20;
	}

	public void UpdateTimer(double seconds)
	{
		if (TimerLabel == null) return;

		var time = TimeSpan.FromSeconds(seconds);

		TimerLabel.Text = $"{((int)time.TotalMinutes).ToString("0")}:{time.Seconds.ToString("00")}:{time.Milliseconds.ToString("00")}";
	}

	public override void _Ready()
	{
		Savedata.Load();
		double pixelationAmount = Savedata.Get<double>("Pixelation", 0);
		
		if (PixelationLayer != null)
		{
			PixelationLayer.Material.Set("shader_parameter/pix", pixelationAmount);

			PixelationLayer.Visible = pixelationAmount > 0;
		}

		Animation?.Play("Show");

		if (Player == null) return;

		Player.Connect("Finished", Callable.From<string>(Finished));
	}

	public override void _Process(double delta) {
		if (Root != null)
			Root.Position = Root.Position.Lerp(-Input.GetLastMouseVelocity()/128, 0.5f);

		if (Player == null) return;

		UpdateTimer(Player.Stopwatch);

		if (IsRestartPressed) 
		{
			ProgressBarProgress += delta * 200.0;
		
			// if the progres bar is full it restarts
			if (ProgressBarProgress >= 100) 
			{
				ResetToCheckpoint();
			}
		}
		else
		{
			ProgressBarProgress -= delta * 250.0;
		}

		ProgressBarProgress = Mathf.Clamp(ProgressBarProgress, 0, 100);
		RestartProgressBar!.Value = ProgressBarProgress;
	}

	public void UpdateSprint(float value, bool animate = true)
	{
		if (SprintBar == null) return;
		
		if (animate)
		{
			GetTree().CreateTween().TweenProperty(SprintBar, "value", value, 0.5);
		}
		else
		{
			SprintBar.Value = value;
		}
	}

	void Finished(string nextLevel) {
		TimerLabel?.Set("theme_override_colors/font_color", Color.Color8(0, 232, 0));

		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().Paused = true;

		if (IntermissionScreen != null && Player != null)
		{
			if (PauseMenu != null) PauseMenu.Visible = false;
			if (Root != null) Root.Visible = false;
			IntermissionScreen.Activate(Player, nextLevel);
		}
	}

	public void ResetToCheckpoint()
	{
		IsRestartPressed = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GetTree().Paused = false;
		ProgressBarProgress = 0;
		EmitSignal("Checkpoint");
	}

	public override void _Input(InputEvent ev)
	{
		if (ev.IsActionPressed("RestartLevel"))
		{
			IsRestartPressed = true;
		}
		else if (ev.IsActionReleased("RestartLevel"))
		{
			IsRestartPressed = false;
		}
	}
}
