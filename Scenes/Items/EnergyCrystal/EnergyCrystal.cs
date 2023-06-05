using Godot;
using System;

public partial class EnergyCrystal : Item
{
	[Export] int ScoreBonus;
	[Export] AnimationPlayer? FloatingAnimation;

	public void PlayerEnter(Node3D body)
	{
		if (body is Player player)
		{
			player.Score += ScoreBonus;
			player.EmitSignal("ScoreUpdated", player.Score);
			QueueFree();
		}
	}

	public override void _Ready()
	{
		FloatingAnimation!.Play("FloatingAnimation");
	}
}
