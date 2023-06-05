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
			var LevelName = GetNode("/root").GetChild(0).Name;

			var seconds = Savedata.Get<double>("Times." + LevelName, 0);
			if (seconds != 0) 
			{
				// Overrides the players previous time if it is faster
				if (player.Stopwatch < seconds)
				{
					Savedata.Set("Times." + LevelName, player.Stopwatch, true);
				}
			}
			else {
				Savedata.Set("Times." + LevelName, player.Stopwatch, true);
			}
		}
	}
}
