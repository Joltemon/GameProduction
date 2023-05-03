using Godot;
using System;

public partial class EnergyCrystal : Node3D
{
	public void playerEnter(Node3D Body)
	{
		if (Body is Player player)
		{
			player.pickUpAmmo();
			GD.Print("goodbye");
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
