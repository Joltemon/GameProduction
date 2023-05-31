using Godot;
using System;

public partial class Deathfield : Area3D
{
	public void DeathfieldEntered(Node3D node) 
	{
		if (node is Player player) 
		{
			player.GoToCheckpoint();
		}
	}
}
