using Godot;
using System;

public partial class MainMenu : Node
{	

	[Export(PropertyHint.File, "*.tscn,*.scn")] string? SettingsPage;
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? CreditsPage;
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? LevelsPage;

    void StartGame() => GD.Print("Game started");

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
