using Godot;
using System;

public partial class Propulsor : Node3D
{
	[Export] PackedScene? Projectile;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("PrimaryFire"))
		{
			
		}
	}
}
