using Godot;
using System;

public partial class Projectile : CharacterBody3D
{
	[Export] public float Speed = 10;
	[Export] PackedScene? WarpField;
	public Vector3 Direction;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var collision = MoveAndCollide(Direction * Speed * (float)delta);

		if (collision != null) // hit a wall, spawn a projectile
		{
			if (WarpField != null)
			{
				var warp = WarpField.Instantiate<WarpField>();
				AddSibling(warp);
				warp.TopLevel = true;
				warp.GlobalPosition = GlobalPosition;
			}

			QueueFree();
		}
	}

	// Destory when out of range
	void OnTimeout() =>	QueueFree();
}
