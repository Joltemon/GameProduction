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

	Vector2 mouseVelocity;

	public override void _Input(InputEvent inputEvent)
	{
		// Shooting the gun
		if (inputEvent.IsActionPressed("PrimaryFire") && (Weapon?.CanFire()??true))
		{
			if (AimRaycast!.IsColliding() && Mathf.Abs(GlobalPosition.DistanceTo(AimRaycast.GetCollisionPoint())) <= 1)
				CloseRangePrimaryFire();
			else
				StandardPrimaryFire();
		}

		if (inputEvent is InputEventMouseMotion mouseEvent)
		{
			mouseVelocity = mouseEvent.Relative;
		}
	}

	public void AddAmmunition(int amount)
	{
		if (Weapon != null)
			Weapon.Ammunition += amount;
	}

	public void SetAmmunition(int amount)
	{
		if (Weapon != null)
		{
			Weapon.Ammunition = amount;
			OnAmmunitionUpdated(Weapon.Ammunition);
		}
	}

	public override void _Process(double delta)
	{
		if (WeaponOffset != null)
		{
			// Look swaying
			var targetRotation = new Vector3();
			targetRotation.X += mouseVelocity.Y / 256;
			targetRotation.Y += mouseVelocity.X / 256;
			WeaponOffset.Rotation = WeaponOffset.Rotation.Lerp(targetRotation, 0.2f);
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
