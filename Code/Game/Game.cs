using Godot;
using System;

public partial class Game : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Settings.SettingsChanged += OnSettingsChanged;
	}

	void OnSettingsChanged()
	{
		if (Savedata.Get<bool>("FullScreen", false))
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
		else
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
	}
}
