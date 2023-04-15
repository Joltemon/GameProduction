using Godot;
using System;

public partial class Propulsor : Node3D
{
	[Export] public PackedScene? Projectile;
	[Export] public Node3D? Tip;

	public Vector3 Target;

	public void PrimaryFire()
	{
		if (Projectile == null) return;

		var shot = Projectile.Instantiate<Projectile>();
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
	}
}
