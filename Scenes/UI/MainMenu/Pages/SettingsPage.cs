using Godot;
using System;

public partial class SettingsPage : Control
{

	[Export] Label? SettingsTitle;
	[Export] Label? ControlsTitle;
	float HSVValue;
	[Export] float Speed;

	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenu; 

	public override void _Process(double delta)
	{
		HSVValue += (float)delta*Speed;
		SettingsTitle?.Set("theme_override_colors/font_outline_color", Color.FromHsv(HSVValue, 1, 1, 1));
		ControlsTitle?.Set("theme_override_colors/font_outline_color", Color.FromHsv(HSVValue, 1, 1, 1));
		if (HSVValue >= 1) {
			HSVValue = 0;
		}
	}

	public override void _Ready() {
		Savedata.Load();
	}

	void BackButtonPressed() {
		if (MainMenu != null) {
			GetTree().ChangeSceneToFile(MainMenu);
		}
	}

	void ApplySettings() {
		Savedata.Save();
	}
}
