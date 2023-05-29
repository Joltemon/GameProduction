using Godot;
using System;

public partial class Endpoint : Node3D
{
	public void EndPointEntered(Node3D node) 
	{
		if (node is Player player) 
		{
			player.StopwatchRunning = false;
			player.EmitSignal("Finished");
		}
	}
}
