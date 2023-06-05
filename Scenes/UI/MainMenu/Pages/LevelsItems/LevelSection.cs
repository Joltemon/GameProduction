using Godot;
using System;

public partial class LevelSection : Panel
{
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? Level;

	[Export] string LevelName = "";

	[Export] Label? TimerLabel;

	public override void _Ready()
	{
		var seconds = Savedata.Get<double>("Times." + LevelName, 0);
		var time = TimeSpan.FromSeconds(seconds);

		TimerLabel!.Text = $"{((int)time.TotalMinutes).ToString("0")}:{time.Seconds.ToString("00")}:{time.Milliseconds.ToString("00")}";
	}

	void LevelPressed()
	{
		GetTree().ChangeSceneToFile(Level);
	}
}
