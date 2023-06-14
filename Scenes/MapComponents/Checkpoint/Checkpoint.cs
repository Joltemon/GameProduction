using Godot;
using System;

public partial class Checkpoint : Area3D
{
	[Export] int CheckpointNumber = 1;

	public void CheckpointEntered(Node3D node) 
	{
		if (node is Player player) 
		{
			if (player.LastCheckpointValue >= CheckpointNumber) return;
			
			player.LastCheckpoint = this;
			player.LastCheckpointValue = CheckpointNumber;
			player.EmitSignal("HitCheckpoint");

			// Save player variables
			player.SavedScore = player.Score;
			player.SavedSprintEnergy = player.SprintEnergy;
		}
	}
}
