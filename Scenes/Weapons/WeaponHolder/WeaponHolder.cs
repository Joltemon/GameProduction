using Godot;
using System;

public partial class WeaponHolder : Node3D
{
	[Export] Propulsor? Weapon;
	[Export] RayCast3D? AimRaycast;

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("PrimaryFire"))
		{
			if (AimRaycast!.IsColliding() && GlobalPosition.DistanceTo(AimRaycast.GetCollisionPoint()) <= 1)
				CloseRangePrimaryFire();
			else
				StandardPrimaryFire();
		}
	}

	void CloseRangePrimaryFire()
	{
		// Create shot immediately at the collision point
		if (Weapon!.Projectile == null) return;
				
		var shot = Weapon!.Projectile.Instantiate<Projectile>();
		AddChild(shot);
		shot.TopLevel = true;
		shot.GlobalPosition = AimRaycast!.GetCollisionPoint();
		shot.GlobalRotation = GlobalRotation;
	}
	
	void StandardPrimaryFire()
	{
		if (AimRaycast!.IsColliding())
		{
			Weapon!.Target = AimRaycast.GetCollisionPoint();
		}
		else
		{
			Weapon!.Target = Vector3.Zero;
		}
		Weapon.PrimaryFire();
	}
}
