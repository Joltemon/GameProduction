using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Export] Label? AmmoLabel;
	[Export] ProgressBar? SprintBar;
	[Export] GpuParticles2D? sprintingParticle;

	void UpdateAmmunition(int ammo)
	{
		if (AmmoLabel != null)
			AmmoLabel.Text = ammo.ToString();
	}

	public void UpdateSpeedEffects(float currentSpeed) 
	{
		var speedLim = 12.5;
		if (currentSpeed > speedLim && sprintingParticle != null) 
		{
			sprintingParticle.Emitting = true;
		}
		else if (currentSpeed < speedLim && sprintingParticle != null)
		{
			sprintingParticle.Emitting = false;
		}
	}

	public void UpdateSprint(float sprint)
	{
		SprintBar!.Value = sprint;
	}
}
