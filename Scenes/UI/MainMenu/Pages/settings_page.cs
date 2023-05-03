using Godot;
using System;

public partial class settings_page : Control
{

	[Export] Label? SettingsTitle;
	[Export] Label? ControlsTitle;
	float HSVValue;
	[Export] float speed; 

	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenu; 

	public override void _Process(double delta)
	{
		HSVValue += (float)delta*speed;
		SettingsTitle?.Set("theme_override_colors/font_outline_color", Color.FromHsv(HSVValue, 1, 1, 1));
		ControlsTitle?.Set("theme_override_colors/font_outline_color", Color.FromHsv(HSVValue, 1, 1, 1));
		if (HSVValue >= 1) {
			HSVValue = 0;
		}
	}

	void BackButtonPressed() {
		if (MainMenu != null) {
			GetTree().ChangeSceneToFile(MainMenu);
		}
	}

	void ApplySettings() {
		
	}
}
