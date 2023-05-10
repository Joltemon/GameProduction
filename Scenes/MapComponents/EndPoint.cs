using Godot;
using System;

public partial class EndPoint : Node3D
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
