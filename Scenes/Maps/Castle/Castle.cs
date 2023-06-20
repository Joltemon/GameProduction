using Godot;
using System;

public partial class Castle : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		await ToSignal(GetTree(), "process_frame");
		GetNode<MeshInstance3D>("./Player/Head/Camera/PostProcesser").Visible = true;
	}
}
