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
	[Export] RayCast3D? FloorDetector;
	[Export] CollisionShape3D? Collider;

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
		LinearDamp = isGrounded ? GroundDrag : AirDrag;
		if (isGrounded)
			LimitSpeed();
	}

	// Clamp the player's horizontal velocity to an acceptable value
	public void LimitSpeed()
	{
		Vector2 flatVelocity = new Vector2(LinearVelocity.X, LinearVelocity.Z);

		// LengthSquared avoids square-root calculation and runs faster.
		if (flatVelocity.Length() > MaxAirSpeed)
		{
			Vector2 limitedVelocity = flatVelocity.Normalized() * MaxAirSpeed;
			LinearVelocity = new Vector3(limitedVelocity.X, LinearVelocity.Y, limitedVelocity.Y);
		}
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
	}
}
