using Godot;
using System;

public partial class BoostCrystal : Item
{
	[Export] AnimationPlayer? FloatingAnimation;

	public void PlayerEntered(Node3D body)
	{
		if (body is Player player)
		{
			QueueFree();
			player.ExtraSpeedBoost();
		}
	}

	public override void _Ready()
	{
		FloatingAnimation!.Play("FloatingAnimation");
	}
}
