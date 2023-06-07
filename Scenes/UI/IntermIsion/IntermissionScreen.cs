using Godot;
using System;

public partial class IntermissionScreen : Control
{
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenu;
	[Export] Label? TimeLabel;

	public void Activate(Player player)
	{
		Visible = true;

		if (TimeLabel != null)
		{
			var time = TimeSpan.FromSeconds(player.Stopwatch);

			TimeLabel.Text = $"Your time: {((int)time.TotalMinutes).ToString("0")}:{time.Seconds.ToString("00")}:{time.Milliseconds.ToString("00")}";
		}
	}

	void NextLevelPressed()
	{
		GD.Print(">:(");
	}

	void RetryPressed()
	{
		GD.Print("https://www.youtube.com/watch?v=DKF2-zCNJ0A");
	}

	void MainMenuPressed()
	{
		GetTree().ChangeSceneToFile(MainMenu);
	}
}
