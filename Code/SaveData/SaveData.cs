using Godot;
using System;

class Savedata
{
	public static void Init()
	{
		GD.Print(ProjectSettings.GlobalizePath("user://"));
	}
}