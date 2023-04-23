using Godot;
using System;

public partial class OverlayCamera : Camera3D
{
	[Export] Camera3D? TrackingCamera;

	public override void _Process(double delta)
	{
		if (TrackingCamera == null) return;
		
		GlobalPosition = TrackingCamera.GlobalPosition;
		GlobalRotation = TrackingCamera.GlobalRotation;
		Fov = TrackingCamera.Fov;
	}
}
