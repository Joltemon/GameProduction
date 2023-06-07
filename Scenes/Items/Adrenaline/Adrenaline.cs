using Godot;
using System;

public partial class Adrenaline : Item
{
	[Export] int Energy;
	[Export] AnimationPlayer? FloatingAnimation;

	public override void PlayerEnter(Player player)
	{
		player.SprintEnergy += Energy;
	}

	public override void _Ready()
	{
		FloatingAnimation!.Play("FloatingAnimation");
	}
}
