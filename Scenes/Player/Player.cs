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
	[Export] float SprintDepletionRate;
	[Export] float CoyoteTime;

	public bool Flying = false;
	float LookSensitivity = 1;
	float FovIncrease;
	public double Stopwatch;
	float currentCoyoteTime;
	public float SprintEnergy = 100;
	
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
	[Export] AnimationPlayer? Animation;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Animation?.Play("Start");
	}

	public override void _Process(double delta)
	{
		// Crouching
		if (Camera != null)
		{
			if (Input.IsActionPressed("MoveCrouch"))
			{
				Camera.Position = Camera.Position.Lerp(Vector3.Down * CrouchStrength, 8 * (float)delta);
			}
			else
			{
				Camera.Position = Camera.Position.Lerp(Vector3.Zero, 8 * (float)delta);
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
			Camera!.Fov = Mathf.Lerp(Camera.Fov, DefaultFov + FovIncrease, 0.2f);
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

		UpdateTimer(delta);
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
		else if (Input.IsActionPressed("MoveSprint") && SprintEnergy > 0)
		{
			sprintAdjustment = SprintMultiplier;
		}
		
		// Get correct direction to apply speed lines when going forwards
		var cameraDir = Camera!.Transform.Basis.Z;
		var moveDirDirection = new Vector2(Camera.Rotation.X, Camera.Rotation.Z).Angle();
		var velocityDirection = new Vector2(LinearVelocity.X, LinearVelocity.Z).Angle();
		var relativeMoveAngle = (Mathf.Abs(moveDirDirection) - Mathf.Abs(velocityDirection));

		// GD.Print(relativeMoveAngle);
		// GD.Print(LinearVelocity < Camera!.Transform.Basis.Z);

		// Apply speed lines and fov increase at higher speeds
		Hud?.UpdateSpeedEffects(currentVelocity);
		FovIncrease = Mathf.Clamp(currentVelocity, 0, 20); //Mathf.Clamp(currentVelocity - 5, 0, 20);

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
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward").Rotated(-Camera!.GlobalRotation.Y);
		Vector3 moveDir = new Vector3(inputDir.X, 0, inputDir.Y) * speed * delta * 180;

		if (speed > 1 && inputDir != Vector2.Zero)
		{
			SprintEnergy -= (float)delta * SprintDepletionRate;
			SprintEnergy = Mathf.Clamp(SprintEnergy, 0, 100);
		}

		// Apply movement speed
		if (isGrounded)
		{
			moveDir *= MoveSpeed;

			LinearDamp = GroundDrag;
		}
		else // Air movement
		{
			moveDir *= AirMoveSpeed;

			// Limit speed
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

		var friction = inputDir.IsZeroApprox() ? 1 : 0;
		PhysicsMaterialOverride.Friction = friction;

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
		
		if (isOnFloor)
			currentCoyoteTime = CoyoteTime;
		else
			currentCoyoteTime -= delta;

		var canJump = currentCoyoteTime > 0;

		if (jumpPressed && canJump && !IsCurrentlyJumping)
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
		moveDir = moveDir.Rotated(Vector3.Right, Camera!.GlobalRotation.X);
		moveDir = moveDir.Rotated(Vector3.Up, Camera!.GlobalRotation.Y);

		moveDir *= FlyMoveSpeed * speed * delta * 180;

		ApplyCentralForce(moveDir);

		// Apply ground drag
		LinearDamp = 15;
	}

	void UpdateTimer(double delta)
	{
		Stopwatch += delta;
		Hud?.UpdateTimer(Stopwatch);
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

			Vector3 newRotation = Camera!.GlobalRotation;
			newRotation.X -= Mathf.DegToRad(mouseMovement.Y * LookSpeed);
			newRotation.X = Mathf.Clamp(newRotation.X, -Mathf.Pi/2, Mathf.Pi/2);
			newRotation.Y -= Mathf.DegToRad(mouseMovement.X * LookSpeed);
			Camera!.GlobalRotation = newRotation;
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
				GetNode<ColorRect>("HUD/HUD/PixelationLayer").Visible = !GetNode<ColorRect>("HUD/HUD/PixelationLayer").Visible;
		}
	}
}
