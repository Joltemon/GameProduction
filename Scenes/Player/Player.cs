using Godot;
using System;

public partial class Player : RigidBody3D
{
	[ExportCategory("Movement")]
	[Export] public float MoveSpeed;
	[Export] public float AirMoveSpeed;
	[Export] public float LookSpeed;
	[Export] public float JumpStrength;
	[Export] public float GroundDrag;
	[Export] public float AirDrag;
	[Export] public float SprintMultiplier;
	[Export] public float CrouchStrength;
	[Export] public float MaxAirSpeed;

	float ColliderTargetHeight;
	float LookPivotTargetHeight;

	[ExportGroup("Player Components")]
	[Export] Camera3D? Camera;
	[Export] ShapeCast3D? FloorDetector;
	[Export] CollisionShape3D? Collider;

	[Signal] public delegate void PrimaryFireEventHandler(Vector3 targetPos);

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{
		// Jump
		if (Input.IsActionJustPressed("MoveJump") && FloorDetector!.IsColliding())
		{
			ApplyCentralImpulse(Vector3.Up * JumpStrength);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 InputDir;
		Vector3 MoveDir;
		float sprintAdjustment = 1;
		bool isGrounded = FloorDetector!.IsColliding();

		InputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward").Rotated(-Camera!.Rotation.Y);
		
		// Crouching (broken right now, don't ask why)
		if (Input.IsActionPressed("MoveCrouch"))
		{
			sprintAdjustment = 0.2f;
			// Collider!.Shape.Set("height", ColliderTargetHeight - CrouchStrength);
			// Collider.Position = new Vector3(Collider.Position.X, (ColliderTargetHeight / 2) - (CrouchStrength / 2), Collider.Position.X);
		}
		else
		{
			// Collider!.Shape.Set("height", ColliderTargetHeight);
			// Collider.Position = new Vector3(Collider.Position.Z, ColliderTargetHeight / 2, Collider.Position.Z);
		}

		// Walking
		if (Input.IsActionPressed("MoveSprint"))
			sprintAdjustment = SprintMultiplier;
		
		MoveDir = new Vector3(InputDir.X, 0, InputDir.Y) * sprintAdjustment * (float)delta * 180;
		
		// Apply movement speed
		MoveDir *= isGrounded ? MoveSpeed : AirMoveSpeed;
		
		ApplyCentralForce(MoveDir);

		// Apply ground drag
		LinearDamp = isGrounded ? GroundDrag : 0;
		// use custom drag formula for air, to preserve gravity
		if (!isGrounded)
			ApplyAirDrag(delta);
	}

	// Apply drag to all axes but Y, to leave gravity acceleration intact
	public void ApplyAirDrag(double delta)
	{
		var newVelocity = LinearVelocity;
		newVelocity.X *= (float)(1 - delta * AirDrag);
		newVelocity.Z *= (float)(1 - delta * AirDrag);
		LinearVelocity = newVelocity;
	}
	
	public override void _Input(InputEvent inputEvent)
	{
		// Mouse Look
		if (inputEvent is InputEventMouseMotion mouseEvent)
		{
			Vector3 newRotation = Camera!.Rotation;
			newRotation.X -= Mathf.DegToRad(mouseEvent.Relative.Y * LookSpeed);
			newRotation.X = Mathf.Clamp(newRotation.X, -Mathf.Pi/2, Mathf.Pi/2);
			newRotation.Y -= Mathf.DegToRad(mouseEvent.Relative.X * LookSpeed);
			Camera!.Rotation = newRotation;
		}
		else if (inputEvent.IsActionPressed("Pause"))
		{
			if (OS.HasFeature("editor"))
				GetTree().Quit();
		}
	}
}
