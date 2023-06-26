using Godot;
using System;

public partial class Item : Area3D
{
	bool Active = true;
	bool BoundSignals;
	
	void OnEnter(Node3D body)
	{
		if (body is Player player)
		{
			BindSignals(player);
			if (Active)
				PlayerEnter(player);
			Remove();
		}
	}
	
	void Remove()
	{
		Active = false;
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
		Active = true;
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
