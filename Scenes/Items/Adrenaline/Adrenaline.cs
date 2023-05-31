using Godot;
using System;

public partial class Adrenaline : Item
{
	[Export] int Energy;
	[Export] AnimationPlayer? FloatingAnimation;

	public void PlayerEnter(Node3D body)
	{
		if (body is Player player)
		{
			player.SprintEnergy += Energy;
			QueueFree();
		}
	}

	public override void _Ready()
	{
		FloatingAnimation!.Play("FloatingAnimation");
	}
}
