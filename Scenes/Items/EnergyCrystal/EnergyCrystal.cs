using Godot;
using System;

public partial class EnergyCrystal : Item
{
	[Export] float ScoreBonus;
	[Export] AnimationPlayer? FloatingAnimation;

	public void PlayerEnter(Node3D body)
	{
		if (body is Player player)
		{
			player.Score += ScoreBonus;
			QueueFree();
		}
	}

	public override void _Ready()
	{
		FloatingAnimation!.Play("FloatingAnimation");
	}
}
