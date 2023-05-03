using Godot;
using System;

public partial class EnergyCrystal : Node3D
{
	[Export] int AmmoAmount;

	public void PlayerEnter(Node3D body)
	{
		if (body is Player player)
		{
			player.WeaponHolder?.AddAmmunition(AmmoAmount);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
