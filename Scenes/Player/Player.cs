using Godot;
using System;

public partial class Player : RigidBody3D
{
	[ExportCategory("Movement")]
	[Export] float MoveSpeed;
	[Export] float AirMoveSpeed;
	[Export] float FlyMoveSpeed;
	[Export] float LookSpeed;
	[Export] float JumpStrength;
	[Export] float GroundDrag;
	[Export] float AirDrag;
	[Export] float Gravity;
	[Export] float SprintMultiplier;
	[Export] float CrouchStrength;
	[Export] float MaxAirSpeed;

	public bool Flying = false;
	float LookSensitivity = 1;
	
	[ExportGroup("CameraOptions")]
	[Export] public float DefaultFov = 75;
	[Export] public float ZoomedFov = 30;

	[ExportGroup("Player Components")]
	[Export] Camera3D? Camera;
	[Export] AnimationPlayer? MoveAnim;
	[Export] ShapeCast3D? FloorDetector;
	[Export] CollisionShape3D? Collider;
	[Export] public WeaponHolder? WeaponHolder;
	[Export] HUD? Hud;

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
		
		if (Input.IsActionPressed("PixelateUp"))
		{
			if (OS.HasFeature("editor"))
				GetNode<ColorRect>("HUD/PixelationLayer").Material.Set("shader_parameter/pix", (int)GetNode<ColorRect>("HUD/PixelationLayer").Material.Get("shader_parameter/pix")+1);
		}
		if (Input.IsActionPressed("PixelateDown"))
		{
			if (OS.HasFeature("editor"))
				GetNode<ColorRect>("HUD/PixelationLayer").Material.Set("shader_parameter/pix", (int)GetNode<ColorRect>("HUD/PixelationLayer").Material.Get("shader_parameter/pix")-1);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float sprintAdjustment = 1;
		bool isGrounded = FloorDetector!.IsColliding();
		var currentVelocity = LinearVelocity.Length();
		
		// Crouching
		if (Input.IsActionPressed("MoveCrouch"))
		{
			sprintAdjustment = 0.2f;
		}
		else if (Input.IsActionPressed("MoveSprint"))
		{
			sprintAdjustment = SprintMultiplier;
		}
		if (Hud != null) 
		{
			Hud.UpdateSpeedEffects(currentVelocity);
		}

		if (Flying)
			MoveFly(isGrounded, sprintAdjustment, (float)delta);
		else
			Move(currentVelocity, isGrounded, sprintAdjustment, (float)delta);
	}

	bool IsCurrentlyJumping;
	void Move(float currentVelocity, bool isGrounded, float speed, float delta)
	{
		GravityScale = Gravity;

		// Walking
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward").Rotated(-Camera!.Rotation.Y);
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

			// Angle calculation
			var moveDirDirection = new Vector2(moveDir.X, moveDir.Z).Angle();
			var velocityDirection = new Vector2(LinearVelocity.X, LinearVelocity.Z).Angle();
			var relativeMoveAngle = (Mathf.Abs(velocityDirection) - Mathf.Abs(moveDirDirection));


			var controlReduction = Mathf.Clamp(currentVelocity, 0, MaxAirSpeed) / MaxAirSpeed; // scale AirMoveSpeed based on how fast the player is going, 0 to 1
			controlReduction *= 1 - Mathf.Abs(relativeMoveAngle / Mathf.Pi); // scale again based on the direction the player is trying to move, going back easier than going forwards

			moveDir *= Mathf.Clamp(1 - controlReduction, 0, 1);

			LinearDamp = 0;

			ApplyAirDrag(delta);
		}
		
		ApplyCentralForce(moveDir);

		// Head bob
		if (MoveAnim != null && isGrounded && currentVelocity > 0)
		{
			MoveAnim.Play("Walk");
			MoveAnim.SpeedScale = currentVelocity / 8;
		}
		else
		{
			MoveAnim?.Stop();
		}

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

	void MoveFly(bool isGrounded, float speed, float delta)
	{
		GravityScale = 0;

		// Flying
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
		Vector3 moveDir = new(inputDir.X, 0, inputDir.Y);
		moveDir = moveDir.Rotated(Vector3.Right, Camera!.Rotation.X);
		moveDir = moveDir.Rotated(Vector3.Up, Camera!.Rotation.Y);

		moveDir *= FlyMoveSpeed * speed * delta * 180;

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
		else if (inputEvent.IsActionPressed("MoveFly"))
		{
			Flying = !Flying;
		}
		else if (inputEvent.IsActionPressed("Pause"))
		{
			if (OS.HasFeature("editor"))
				GetTree().Quit();
		}
		else if (inputEvent.IsActionPressed("Reload"))
		{
			if (OS.HasFeature("editor"))
				WeaponHolder?.SetAmmunition(10);
		}
		else if (inputEvent.IsActionPressed("Pixelate"))
		{
			if (OS.HasFeature("editor"))
				GetNode<ColorRect>("HUD/PixelationLayer").Visible = !GetNode<ColorRect>("HUD/PixelationLayer").Visible;
		}
	}
}
