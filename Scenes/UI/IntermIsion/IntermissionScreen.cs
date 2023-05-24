using Godot;
using System;

public partial class IntermissionScreen : Control
{
	[Export(PropertyHint.File, "*.tscn,*.scn")] string? MainMenu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(";-;");
	}

	void NextLevelPressed(){
		GD.Print(">:(");
	}

	void RetryPressed(){
		GD.Print("https://www.youtube.com/watch?v=DKF2-zCNJ0A");
	}

	void MainMenuPressed(){
		GetTree().ChangeSceneToFile(MainMenu);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
