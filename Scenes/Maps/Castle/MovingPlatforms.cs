using Godot;
using System;

public partial class MovingPlatforms : Area3D
{

	void OnPlayerEntered(Node3D body)
	{
		if (body is Player player)
		{
			player.Reparent(this, true);

		}
	}

	void OnPlayerExited(Node3D body)
	{
		if (body is Player player)
		{
			player.Reparent(GetNode("../../../"), true);
		}
	}
}
