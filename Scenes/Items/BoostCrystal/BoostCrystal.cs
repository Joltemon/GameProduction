using Godot;
using System;

public partial class BoostCrystal : Item
{
	[Export] AnimationPlayer? FloatingAnimation;

	public override void PlayerEnter(Player player)
	{
		player.ExtraSpeedBoost();
	}

	public override void _Ready()
	{
		FloatingAnimation!.Play("FloatingAnimation");
	}
}
