using Godot;
using System;

public partial class EnergyCrystal : Item
{
	[Export] int ScoreBonus;
	[Export] AnimationPlayer? FloatingAnimation;

	public override void PlayerEnter(Player player)
	{
		player.Score += ScoreBonus;
		player.EmitSignal("ScoreUpdated", player.Score);
	}

	public override void _Ready()
	{
		FloatingAnimation!.Play("FloatingAnimation");
	}
}
