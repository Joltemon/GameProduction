using Godot;
using System;

public partial class LevelsPage : Control
{
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenuPage;
	[Export]Control? Galaxy;

	void BackButtonPressed() 
	{
		GetTree().ChangeSceneToFile(MainMenuPage);
	}

	public override void _Process(double delta) 
	{
		if(Galaxy!=null)
		{
			Vector2 OffsetPosition = GetGlobalMousePosition() - Galaxy.Size/2;
			Galaxy.Position = Galaxy.Position.Lerp(OffsetPosition/32, 0.1f);
		}
	}
}
