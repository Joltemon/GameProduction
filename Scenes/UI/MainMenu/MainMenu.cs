using Godot;
using System;

public partial class MainMenu : Node
{	
	void StartGame()
	{
		GD.Print("Game started");
	}

	void Quit()
	{
		GetTree().Quit();
	}
}
