using Godot;
using System;

public partial class EnergyCrystal : Node3D
{
	[Export] int AmmoAmount;
	[Export] AnimationPlayer? FloatingAnimation;

	public void PlayerEnter(Node3D body)
	{
		if (body is Player player)
		{
			player.WeaponHolder?.AddAmmunition(AmmoAmount);
			QueueFree();
		}
	}

	public override void _Ready()
	{
		FloatingAnimation!.Play("FloatingAnimation");
	}
}
