using Godot;
using System;

public partial class PauseMenu : Control
{

	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenu; 

	void ResumeButtonPressed() 
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Visible = false;
		GetTree().Paused = false;
	}

	void MainMenuButtonPressed() 
	{
		if (MainMenu != null) {
			GetTree().ChangeSceneToFile(MainMenu);
			GetTree().Paused = false;
		}
	}

	void RestartButtonPressed()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Visible = false;
		GetTree().Paused = false;
		GetTree().ReloadCurrentScene();
	}


	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("Pause"))
		{
			if (Visible) 
			{
				ResumeButtonPressed();
			}
			else 
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
				Visible = true;
				GetTree().Paused = true;
			}
		}
	}
}
