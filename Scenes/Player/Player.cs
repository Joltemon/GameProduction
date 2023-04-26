using Godot;
using System;

public partial class Player : RigidBody3D
{
	[ExportCategory("Movement")]
	[Export] public float MoveSpeed;
	[Export] public float AirMoveSpeed;
	[Export] public float FlyMoveSpeed;
	[Export] public float LookSpeed;
	[Export] public float JumpStrength;
	[Export] public float GroundDrag;
	[Export] public float AirDrag;
	[Export] public float Gravity;
	[Export] public float SprintMultiplier;
	[Export] public float CrouchStrength;
	[Export] public float MaxAirSpeed;

	public bool Flying = false;
	float LookSensitivity = 1;
	
	[ExportGroup("CameraOptions")]
	[Export] public float DefaultFov = 75;
	[Export] public float ZoomedFov = 30;

	[ExportGroup("Player Components")]
	[Export] Camera3D? Camera;
	[Export] Camera3D? OverlayCamera;
	[Export] ShapeCast3D? FloorDetector;
	[Export] CollisionShape3D? Collider;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Process(double delta)
	{
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

		// Camera zoom
		if (Input.IsActionPressed("LookZoom"))
		{
			Camera!.Fov = Mathf.Lerp(Camera.Fov, ZoomedFov, 0.2f);
			LookSensitivity = 0.4f;
		}
		else
		{
			Camera!.Fov = Mathf.Lerp(Camera.Fov, DefaultFov, 0.2f);
			LookSensitivity = 1;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float sprintAdjustment = 1;
		bool isGrounded = FloorDetector!.IsColliding();

		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward").Rotated(-Camera!.Rotation.Y);
		
		// Crouching
		if (Input.IsActionPressed("MoveCrouch"))
		{
			sprintAdjustment = 0.2f;
		}
		else if (Input.IsActionPressed("MoveSprint"))
		{
			sprintAdjustment = SprintMultiplier;
		}

		if (Flying)
			MoveFly(inputDir, isGrounded, sprintAdjustment, (float)delta);
		else
			Move(inputDir, isGrounded, sprintAdjustment, (float)delta);
	}

	bool IsCurrentlyJumping;
	void Move(Vector2 inputDir, bool isGrounded, float speed, float delta)
	{
		GravityScale = Gravity;

		// Walking
		Vector3 moveDir = new Vector3(inputDir.X, 0, inputDir.Y) * speed * delta * 180;
		
		// Apply movement speed
		if (isGrounded)
		{
			moveDir *= MoveSpeed;

			LinearDamp = GroundDrag;
		}
		else
		{
			moveDir *= AirMoveSpeed;

			var moveDirDirection = new Vector2(moveDir.X, moveDir.Z).Normalized().Angle();
			var velocityDirection = new Vector2(LinearVelocity.X, LinearVelocity.Z).Normalized().Angle();
			var relativeMoveAngle = (velocityDirection - moveDirDirection) % 2*Mathf.Pi;

			var controlReduction = Mathf.Clamp(MaxAirSpeed - LinearVelocity.Length(), 0, MaxAirSpeed) / MaxAirSpeed; // scale AirMoveSpeed based on how fast the player is going
			controlReduction *= Mathf.Abs((Mathf.Pi - Mathf.Abs(relativeMoveAngle)) / Mathf.Pi);

			moveDir *= controlReduction;


			LinearDamp = 0;
			// ApplyAirDrag(delta);
		}
		
		ApplyCentralForce(moveDir);

		// Jumping
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
	}

	void MoveFly(Vector2 inputDir, bool isGrounded, float speed, float delta)
	{
		GravityScale = 0;

		// Flying
		Vector3 moveDir = new Vector3(inputDir.X, 0, inputDir.Y).Rotated(Vector3.Right!, Camera!.Rotation.X) * speed * delta * 180;
		
		// Apply movement speed
		moveDir *= isGrounded ? MoveSpeed : MoveSpeed;
		
		ApplyCentralForce(moveDir);

		// Apply ground drag
		LinearDamp = 15;
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
			var mouseMovement = mouseEvent.Relative * LookSensitivity;

			Vector3 newRotation = Camera!.Rotation;
			newRotation.X -= Mathf.DegToRad(mouseMovement.Y * LookSpeed);
			newRotation.X = Mathf.Clamp(newRotation.X, -Mathf.Pi/2, Mathf.Pi/2);
			newRotation.Y -= Mathf.DegToRad(mouseMovement.X * LookSpeed);
			Camera!.Rotation = newRotation;
		}
		else if (inputEvent.IsActionPressed("Pause"))
		{
			if (OS.HasFeature("editor"))
				GetTree().Quit();
		}
		else if (inputEvent.IsActionPressed("MoveFly"))
		{
			Flying = !Flying;
		}
	}
}
