using Godot;
using System;

public partial class Adrenaline : Node3D
{
	[Export] int Energy;

	public void PlayerEnter(Node3D body)
	{
		if (body is Player player)
		{
			player.SprintEnergy += Energy;
			QueueFree();
		}
	}
}
