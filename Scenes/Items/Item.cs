using Godot;
using System;

public partial class Item : Area3D
{
	bool BoundSignals;
	
	void OnEnter(Node3D body)
	{
		if (body is Player player)
		{
			PlayerEnter(player);
			BindSignals(player);
			Remove();
		}
	}
	
	void Remove()
	{
		SetDeferred("visible", false);
		SetDeferred("monitoring", false);
	}

	// When player hits a checkpoint, remove for real
	void ConfirmRemoval()
	{
		QueueFree();
	}

	// When a player respawns from a checkpoint, reactivate the item
	void Respawn()
	{
		Visible = true;
		Monitoring = true;
	}

	void BindSignals(Player player)
	{
		if (BoundSignals) return;
		else BoundSignals = true;

		player.Connect("HitCheckpoint", Callable.From(ConfirmRemoval));
		player.Connect("RestoredFromCheckpoint", Callable.From(Respawn));
	}

	public virtual void PlayerEnter(Player player) {}
}
