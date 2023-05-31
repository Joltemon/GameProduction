using Godot;
using System;

public partial class Spawnpoint : Node3D
{
	[Export] PackedScene? Player;

	public override async void _Ready()
	{
		await ToSignal(GetTree(), "process_frame");
		var player = Player!.Instantiate<Node3D>();
		AddSibling(player);
		player.GlobalPosition = GlobalPosition;
		player.GlobalRotation = GlobalRotation;
	}
}
