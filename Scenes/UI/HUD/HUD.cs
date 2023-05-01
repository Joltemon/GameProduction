using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[Export] Label? AmmoLabel;

	void AmmunitionUpdated(int ammo)
	{
		if (AmmoLabel != null)
			AmmoLabel.Text = ammo.ToString();
	}
}
