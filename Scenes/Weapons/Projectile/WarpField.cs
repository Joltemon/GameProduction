using Godot;
using System;

public partial class WarpField : Area3D
{
	[Export] float Force;
	[Export] float Radius;

	public override void _PhysicsProcess(double delta)
	{
		var collateral = GetOverlappingBodies();

		// if an object is caught in the blast, launch them
		foreach (var item in collateral)
		{
			if (item is RigidBody3D body)
			{
				// var circleSize = new Vector3(Radius, Radius, Radius);
				var launchDirection = GlobalPosition.DirectionTo(body.GlobalPosition);
				GD.Print(GlobalPosition - body.GlobalPosition);
				
				// calculate inverse distance from body
				var launchStrength = GlobalPosition.DistanceTo(body.GlobalPosition);
				
				if (launchStrength == 0)
					continue; // cannot divide by zero

				launchStrength = Radius - Mathf.Abs(launchStrength);
				
				// body.ApplyCentralImpulse(launchDirection * launchStrength * Force);
				body.ApplyImpulse(launchDirection * launchStrength * Force, GlobalPosition);

				QueueFree();
			}
		}
	}

	// reduce the difference between close knockback (e.g. 0.8 and 0.9 distance shouldn't cause a great difference in propulsion)
	float AdjustedFalloff(float x)
	{
		return -Mathf.Pow(x - 1, 4) + 1;
	}
}
