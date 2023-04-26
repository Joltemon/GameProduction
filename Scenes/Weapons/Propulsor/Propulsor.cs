using Godot;
using System;

public partial class Propulsor : Node3D
{
	[Export] public PackedScene? Projectile;
	[Export] public Node3D? Tip;

	[Export] float VerticalForce;
	[Export] float HorizontalForce;

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

		PlayerRecoil();


	}

	public void PlayerRecoil() 
	{
		var launchDirection = ToGlobal(new Vector3(0,0,-1)).DirectionTo(GlobalPosition);
		var force = new Vector3(launchDirection.X * HorizontalForce, launchDirection.Y * VerticalForce, launchDirection.Z * HorizontalForce);

		GetNode<RigidBody3D>("../../../../../").ApplyImpulse(force, GlobalPosition);

	}
}
