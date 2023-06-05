using Godot;
using System;

public partial class MainMenu : Node
{

	[Export(PropertyHint.File, "*.tscn,*.scn")] string? SettingsPage;
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? CreditsPage;
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? LevelsPage;

	[Export(PropertyHint.File, "*.tscn,*.scn")] string? StartLevel;

	[Export] AnimationPlayer? LevelDisplayAnimation;

	public override void _Ready()
	{
		Savedata.Load();
		LevelDisplayAnimation!.Play("Sway");
	}

    void StartGame()
	{
		GetTree().ChangeSceneToFile(StartLevel);
	}

    void Quit() => GetTree().Quit();

    void SettingsPressed() {
		if (SettingsPage != null) {
			GetTree().ChangeSceneToFile(SettingsPage);
		}
	}

    void CreditsPressed() {
		if (CreditsPage != null) {
			GetTree().ChangeSceneToFile(CreditsPage);
		}
	}

	void LevelsButtonPressed() 
	{
		if (LevelsPage != null) {
			GetTree().ChangeSceneToFile(LevelsPage);
		}
	}

}
