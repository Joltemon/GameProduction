using Godot;
using System;

public partial class Spawnpoint : Node3D
{
	[Export] PackedScene? Player;

	public override void _Ready()
	{
		var player = Player!.Instantiate<Node3D>();
		player.GlobalPosition = GlobalPosition;
		player.GlobalRotation = GlobalRotation;
		AddSibling(player);
	}
}
