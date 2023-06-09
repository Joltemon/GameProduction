using Godot;
using System;

public partial class WeaponHolder : Node3D
{
	[Export] Player? Player;
	[Export] Propulsor? Weapon;
	[Export] Node3D? WeaponOffset;
	[Export] RayCast3D? AimRaycast;
	[Export] AnimationPlayer? AnimationPlayer;

	[Export] float VerticalRecoil;
	[Export] float HorizontalRecoil;

	[Signal] public delegate void FiredEventHandler();
	[Signal] public delegate void AmmunitionUpdatedEventHandler(int ammunition);

	[Export] double ShootingCoyoteTime;
	double CurrentShootingCoyoteTime;

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("PrimaryFire"))
		{
			CurrentShootingCoyoteTime = ShootingCoyoteTime;
		}
	}

	public override void _Process(double delta)
	{
		// Shooting the gun
		if (CurrentShootingCoyoteTime > 0 && (Weapon?.CanFire()??true))
		{
			CurrentShootingCoyoteTime = 0;

			if (AimRaycast!.IsColliding() && Mathf.Abs(GlobalPosition.DistanceTo(AimRaycast.GetCollisionPoint())) <= 1)
				CloseRangePrimaryFire();
			else
				StandardPrimaryFire();
		}
		
		if (WeaponOffset != null)
		{
			// Look swaying
			var targetRotation = new Vector3();
			var mouseVelocity = Input.GetLastMouseVelocity();
			targetRotation.X = Mathf.DegToRad(mouseVelocity.Y / 128);
			targetRotation.Y = Mathf.DegToRad(mouseVelocity.X / 128);
			WeaponOffset.Rotation = WeaponOffset.Rotation.Lerp(targetRotation, 0.1f);
		}

		CurrentShootingCoyoteTime -= delta;
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
		AnimationPlayer?.Stop();
		AnimationPlayer?.Play("Fire");
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

	public void OnFired()
	{
		EmitSignal("Fired");
		var launchDirection = ToGlobal(new Vector3(0,0,-1)).DirectionTo(GlobalPosition);
		var force = new Vector3(launchDirection.X * HorizontalRecoil, launchDirection.Y * VerticalRecoil, launchDirection.Z * HorizontalRecoil);

		Player?.ApplyImpulse(force, GlobalPosition);
	}

	void OnAmmunitionUpdated(int ammo) => EmitSignal("AmmunitionUpdated", ammo);
}
