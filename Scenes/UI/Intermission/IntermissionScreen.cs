using Godot;
using System;

public partial class IntermissionScreen : Control
{
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenu;
	[Export] Label? TimeLabel;
	string? NextLevel;

	public void Activate(Player player, string nextLevel)
	{
		NextLevel = nextLevel;
		Visible = true;

		if (TimeLabel != null)
		{
			var time = TimeSpan.FromSeconds(player.Stopwatch);

			TimeLabel.Text = $"Your time: {(int)time.TotalMinutes:0}:{time.Seconds:00}:{time.Milliseconds:00}";
		}
	}

	void NextLevelPressed()
	{
		GetTree().Paused = false;
		if (!String.IsNullOrWhiteSpace(NextLevel))
		{
			GetTree().ChangeSceneToFile(NextLevel);
		}
		else
		{
			GetTree().ChangeSceneToFile(MainMenu);
		}
	}

	void RetryPressed()
	{
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}

	void MainMenuPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile(MainMenu);
	}
}
