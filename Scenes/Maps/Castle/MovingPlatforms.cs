using Godot;
using System;

public partial class MovingPlatforms : Area3D
{
	bool IsPlayerInside = false;
	bool IsMovingPlayer = false;
	Node3D? OldParent;

	async void OnPlayerEntered(Node3D body)
	{
		if (body is Player player && !IsPlayerInside && !IsMovingPlayer)
		{
			IsMovingPlayer = true;

			OldParent = player.GetParent<Node3D>();

			player.Reparent(this, true);

			IsPlayerInside = true;

			await ToSignal(GetTree(), "process_frame");
			IsMovingPlayer = false;
		}
	}

	async void OnPlayerExited(Node3D body)
	{
		if (body is Player player && IsPlayerInside && !IsMovingPlayer)
		{
			IsMovingPlayer = true;
			
			if (OldParent != null)
				player.Reparent(OldParent, true);

			IsPlayerInside = false;

			await ToSignal(GetTree(), "process_frame");
			IsMovingPlayer = false;
		}
	}
}
