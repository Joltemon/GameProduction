using Godot;
using System;

public partial class Checkpoint : Area3D
{
	[Export] int CheckpointNumber;

	public void CheckpointEntered(Node3D node) 
	{
		if (node is Player player) 
		{
			player.LastCheckpoint = GlobalPosition;
			player.LastCheckpointValue = CheckpointNumber;
		}
	}
}
