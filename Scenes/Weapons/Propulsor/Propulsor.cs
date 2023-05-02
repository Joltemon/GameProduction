using Godot;
using System;

public partial class Propulsor : Node3D
{
	[Export] public int Ammunition;

	[ExportGroup("Components")]
	[Export] public PackedScene? Projectile;
	[Export] public Node3D? Tip;
	[Export] public Timer? CooldownTimer;
	[Signal] public delegate void FiredEventHandler();
	[Signal] public delegate void AmmunitionUpdatedEventHandler(int ammunition);

	public Vector3 Target;

	public override void _Ready()
	{
		EmitSignal("AmmunitionUpdated", Ammunition);
	}

	public void PrimaryFire()
	{
		if (!CanFire()) return;
		else
		{
			Ammunition--;
			EmitSignal("AmmunitionUpdated", Ammunition);
			CooldownTimer?.Start();
		}

		var shot = Projectile!.Instantiate<Projectile>();
		AddChild(shot);
		shot.TopLevel = true;
		shot.GlobalPosition = Tip!.GlobalPosition;
		shot.GlobalRotation = Tip!.GlobalRotation;

		if (Target != Vector3.Zero)
		{
			shot.Direction = shot.GlobalPosition.DirectionTo(Target);
		}
		else
		{
			shot.Direction = shot.GlobalTransform.Basis.X;
		}

		EmitSignal("Fired");
	}

	public bool CanFire()
	{
		// Sanity checks
		if (Projectile == null) return false;

		if (!CooldownTimer?.IsStopped()??false) return false;

		if (Ammunition <= 0) return false;

		return true;
	}
}
