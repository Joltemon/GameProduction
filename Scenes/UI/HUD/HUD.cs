using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Export] Label? AmmoLabel;

	[Export] GpuParticles2D? sprintingParticle;

	void AmmunitionUpdated(int ammo)
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
}
