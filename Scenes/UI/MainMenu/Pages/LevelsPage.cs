using Godot;
using System;

public partial class LevelsPage : Control
{
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenuPage;

	void BackButtonPressed() 
	{
		GetTree().ChangeSceneToFile(MainMenuPage);
	}
}
