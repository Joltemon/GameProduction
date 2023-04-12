using Godot;
using System;

public partial class Projectile : Area3D
{
	[Export] float Speed;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Position += Vector3.Forward * Speed;
	}
}
