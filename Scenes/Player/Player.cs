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

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	bool IsCurrentlyJumping;
	public override void _Process(double delta)
	{
		// Jump
		var jumpPressed = Input.IsActionPressed("MoveJump");
		var isOnFloor = FloorDetector!.IsColliding();

		if (jumpPressed && isOnFloor && !IsCurrentlyJumping)
		{
			// Jumping
			IsCurrentlyJumping = true;
			ApplyCentralImpulse(Vector3.Up * JumpStrength);
		}
		else if (isOnFloor && IsCurrentlyJumping)
		{
			// Just landed
			IsCurrentlyJumping = false;
		}

		// Crouching
		if (Camera != null)
		{
			if (Input.IsActionPressed("MoveCrouch"))
			{
				Camera.Position = Camera.Position.Lerp(Vector3.Down * CrouchStrength, 16 * (float)delta);
			}
			else
			{
				Camera.Position = Camera.Position.Lerp(Vector3.Zero, 16 * (float)delta);
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// GD.Print(GlobalPosition.Y);
		Vector2 InputDir;
		Vector3 MoveDir;
		float sprintAdjustment = 1;
		bool isGrounded = FloorDetector!.IsColliding();

		InputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward").Rotated(-Camera!.Rotation.Y);
		
		// Crouching (broken right now, don't ask why)
		if (Input.IsActionPressed("MoveCrouch"))
		{
			sprintAdjustment = 0.2f;
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
	void ApplyAirDrag(double delta)
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
