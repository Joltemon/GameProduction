using Godot;
using System;

public partial class MainMenu : Node
{	

	[Export] PackedScene? SettingsPage;

	void StartGame()
	{
		GD.Print("Game started");
	}

	void Quit()
	{
		GetTree().Quit();
	}

	void SettingsPressed() {
		if (SettingsPage != null) {
			GetTree().ChangeSceneToPacked(SettingsPage);
		}
	}
}
