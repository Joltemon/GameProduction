using Godot;
using System;

public partial class OverlayCamera : Camera3D
{
	[Export] Node3D? TrackingNode;

	public override void _Process(double delta)
	{
		if (TrackingNode == null) return;
		
		GlobalPosition = TrackingNode.GlobalPosition;
		GlobalRotation = TrackingNode.GlobalRotation;
	}
}
