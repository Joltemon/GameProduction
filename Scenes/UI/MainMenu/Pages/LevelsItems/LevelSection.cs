using Godot;
using System;

public partial class LevelSection : Panel
{
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? Level;

	void LevelPressed()
	{
		GetTree().ChangeSceneToFile(Level);
	}
}
