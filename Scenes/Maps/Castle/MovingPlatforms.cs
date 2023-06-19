using Godot;
using System;

public partial class MovingPlatforms : Area3D
{
	private bool timeout = false;
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
			if (timeout == true)
			{
				timeout = false;
				player.Reparent(GetNode("../../../"), true);
			}
		}
	}

	void OnTimerTimeout()
	{
		timeout = true;
	}
}
