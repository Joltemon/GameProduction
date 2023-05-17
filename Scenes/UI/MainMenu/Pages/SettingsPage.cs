using Godot;
using System;

public partial class SettingsPage : Control
{
	[Export] Control? SettingsPanel;
	[Export] Label? SettingsTitle;
	[Export] Label? ControlsTitle;
	float HSVValue;
	[Export] float Speed;

	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenu; 

	public override void _Process(double delta)
	{
		HSVValue += (float)delta*Speed;

		if (HSVValue >= 1)
			HSVValue = 0;

		SettingsTitle?.Set("theme_override_colors/font_outline_color", Color.FromHsv(HSVValue, 1, 1, 1));
		ControlsTitle?.Set("theme_override_colors/font_outline_color", Color.FromHsv(HSVValue, 1, 1, 1));
	}

	public override void _Ready() {
		LoadSettings();
	}

	void BackButtonPressed() {
		if (MainMenu != null) {
			GetTree().ChangeSceneToFile(MainMenu);
		}
	}

	void LoadSettings()
	{
		if (SettingsPanel == null) return;

		Savedata.Load();

		foreach (var item in SettingsPanel.GetChildren())
		{
			if (item is ISettingsItem settingsItem)
			{
				settingsItem.SetValue(Savedata.Get(settingsItem.GetName()));
			}
		}
	}

    void ApplySettings()
	{
		if (SettingsPanel == null) return;

		foreach (var item in SettingsPanel.GetChildren())
		{
			if (item is ISettingsItem settingsItem)
			{
				Savedata.Set(settingsItem.GetName(), settingsItem.GetValue());
			}
		}

		Savedata.Save();
	}
}
