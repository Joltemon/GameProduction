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
	[Export] float CrouchSlideBoost;

	public bool Flying = false;
	float LookSensitivity = 1;
	float FovIncrease;
	public double Stopwatch;
	public Boolean StopwatchRunning = true;
	public int Score;
	public int SavedScore;
	float CurrentCoyoteTime;
	float sprintEnergy = 100;
	public float SavedSprintEnergy = 100;
	public Node3D? LastCheckpoint;
	public int LastCheckpointValue;
	public float SprintEnergy
	{
		get => sprintEnergy;
		set
		{
			sprintEnergy = value;
			Hud?.UpdateSprint(value);
		}
	}

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

	[Export] float ExtraSpeedMaxTime;
	double ExtraSpeedTimer;
	Boolean ExtraSpeedChange = false;

	[Signal] public delegate void FinishedEventHandler(string nextLevel);
	[Signal] public delegate void ScoreUpdatedEventHandler(int score);
	[Signal] public delegate void HitCheckpointEventHandler();
	[Signal] public delegate void RestoredFromCheckpointEventHandler();

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		Animation?.Play("Start");
		LastCheckpoint = GetParent<Node3D>();

		Savedata.Load();
		if (Savedata.Get<bool>("FullScreen", false))
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
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

		UpdateTimer(delta);
		

		if (ExtraSpeedTimer > 0) 
		{
			ExtraSpeedTimer -= delta * 200.0;
			Hud!.ExtraSpeedBar!.Value = ExtraSpeedTimer;
			if (ExtraSpeedChange == false)
			{
				ExtraSpeedChange = true;
				UpdateSpeed(1.5f);
				Hud.SprintingParticle!.ProcessMaterial.Set("color", new Color(0.949f, 0, 0.968f, 1));
			}
		}
		else 
		{
			Hud!.ExtraSpeedBar!.Visible = false;
			if (ExtraSpeedChange == true)
			{
				ExtraSpeedChange = false;
				UpdateSpeed(1/1.5f);
				Hud.SprintingParticle!.ProcessMaterial.Set("color", Color.Color8(255, 255, 255));
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float sprintAdjustment = 1;
		bool isGrounded = FloorDetector!.IsColliding();
		var currentVelocity = LinearVelocity.Length();

		// Sprinting
		if (Input.IsActionPressed("MoveSprint") && SprintEnergy > 0)
		{
			sprintAdjustment = SprintMultiplier;
		}

		// Get correct direction to apply speed lines when going forwards
		var cameraDir = Camera!.Transform.Basis.Z;
		var moveDirDirection = new Vector2(Camera.Rotation.X, Camera.Rotation.Z).Angle();
		var velocityDirection = new Vector2(LinearVelocity.X, LinearVelocity.Z).Angle();
		var relativeMoveAngle = (Mathf.Abs(moveDirDirection) - Mathf.Abs(velocityDirection));
		
		// Apply speed lines and fov increase at higher speeds
		FovIncrease = Mathf.Clamp(currentVelocity, 0, 20);
		Hud?.UpdateSpeedEffects(currentVelocity, (LinearVelocity.Normalized().Dot(-Camera!.GlobalTransform.Basis.Z)));


		if (Flying)
			MoveFly(sprintAdjustment, (float)delta);
		else
			Move(currentVelocity, isGrounded, sprintAdjustment, (float)delta);
	}

	bool IsCurrentlyCrouchSliding;
	void Move(float currentVelocity, bool isGrounded, float speed, float delta)
	{
		GravityScale = Gravity;

		// Crouching
		var crouching = false;
		if (Input.IsActionPressed("MoveCrouch"))
		{
			crouching = true;
			speed = 0.2f;
		}

		// Walking
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward").Rotated(-Camera!.GlobalRotation.Y);
		Vector3 moveDir = new Vector3(inputDir.X, 0, inputDir.Y) * speed * delta * 180;

		if (speed > 1 && inputDir != Vector2.Zero)
		{
			sprintEnergy -= (float)delta * SprintDepletionRate;
			sprintEnergy = Mathf.Clamp(SprintEnergy, 0, 100);
			Hud?.UpdateSprint(sprintEnergy, false);
		}

		// Apply movement speed
		var isCrouchSliding = crouching && currentVelocity >= 5;

		if (isGrounded)
		{
			if (isCrouchSliding)
			{
				LinearDamp = 0.3f;
				
				if (!IsCurrentlyCrouchSliding)
				{
					ApplyCentralImpulse(moveDir * CrouchSlideBoost); // Add boost when first crouch sliding
					IsCurrentlyCrouchSliding = true;
				}
			}
			else
			{
				IsCurrentlyCrouchSliding = false;

				moveDir *= MoveSpeed;
				LinearDamp = GroundDrag;
				
				var friction = inputDir.IsZeroApprox() ? 1 : 0;
				PhysicsMaterialOverride.Friction = friction;
			}
		}
		else // Air movement
		{
			moveDir *= AirMoveSpeed;

			// Limit speed
			// Angle calculation, desired movement direction relative to their current movement direction
			var moveDirDirection = new Vector2(moveDir.X, moveDir.Z).Angle();
			var velocityDirection = new Vector2(LinearVelocity.X, LinearVelocity.Z).Angle();
			var relativeMoveAngle = (Mathf.Abs(velocityDirection) - Mathf.Abs(moveDirDirection));

			var controlReduction = Mathf.Clamp(currentVelocity, 0, MaxAirSpeed) / MaxAirSpeed; // scale speed based on how fast the player is going, 0 to 1
			controlReduction *= 1 - Mathf.Abs(relativeMoveAngle / Mathf.Pi); // scale again based on the direction the player is trying to move, going back easier than going forwards
			// controlReduction = Mathf.Pow(controlReduction, 2); // adjusts controlreduction to be more generous when moving slowly

			moveDir *= Mathf.Clamp(1 - controlReduction, 0, 1);

			LinearDamp = 0;

			ApplyAirDrag(delta);

			PhysicsMaterialOverride.Friction = 0; // Set friction to 0 in air

			// Push the player down when crouching in the air
			if (crouching)
			{
				ApplyCentralForce(Vector3.Down * CrouchSlideBoost);
			}
		}

		ApplyCentralForce(moveDir);

		HandleJump(delta, crouching);

		// Head bob
		if (MoveAnim != null && isGrounded && currentVelocity > 0 && !isCrouchSliding)
		{
			MoveAnim.Play("Walk");
			MoveAnim.SpeedScale = currentVelocity / 8;
		}
		else
		{
			MoveAnim?.Stop();
		}
	}

	bool IsCurrentlyJumping;
	void HandleJump(float delta, bool crouching)
	{
		// Jumping
		var jumpPressed = Input.IsActionPressed("MoveJump");
		var isOnFloor = FloorDetector!.IsColliding() && FloorDetector.GetCollisionNormal(0).Y > 0.4;

		if (isOnFloor && !IsCurrentlyJumping)
		{
			CurrentCoyoteTime = CoyoteTime;
		}
		else
		{
			CurrentCoyoteTime -= delta;
		}

		CurrentCoyoteTime = Mathf.Clamp(CurrentCoyoteTime, 0, CoyoteTime);

		var canJump = CurrentCoyoteTime > 0;

		if (jumpPressed && canJump && !IsCurrentlyJumping)
		{
			// Jumping
			IsCurrentlyJumping = true;
			CurrentCoyoteTime = 0;

			if (LinearVelocity.Y < 0)
			{
				LinearVelocity *= new Vector3(1, 0, 1);
			}
			
			if (crouching)
			{
				ApplyCentralImpulse(Vector3.Up * JumpStrength / 2);
			}
			else
			{
				ApplyCentralImpulse(Vector3.Up * JumpStrength);
			}
		}
		else if ((!isOnFloor && IsCurrentlyJumping) || !jumpPressed)
		{
			// In the air
			IsCurrentlyJumping = false;
		}
	}

	void MoveFly(float speed, float delta)
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
		if (StopwatchRunning == true)
		{
			Stopwatch += delta;
		}
	}

	// Apply drag to all axes but Y, to leave gravity acceleration intact
	void ApplyAirDrag(double delta)
	{
		var newVelocity = LinearVelocity;
		newVelocity.X *= (float)(1 - delta * AirDrag);
		newVelocity.Z *= (float)(1 - delta * AirDrag);
		LinearVelocity = newVelocity;
	}

	public async void GoToCheckpoint()
	{
		LinearVelocity = Vector3.Zero;

		Animation?.Stop();
		Animation?.Play("Start");
		Freeze = true;
		await ToSignal(GetTree(), "process_frame");
		GlobalPosition = LastCheckpoint?.GlobalPosition??Vector3.Zero;
		GlobalRotation = Vector3.Zero;
		Camera!.GlobalRotation = LastCheckpoint?.GlobalRotation??Vector3.Zero;
		
		Freeze = false;

		if (LastCheckpointValue == 0)
		{
			Stopwatch = 0;
		}

		EmitSignal("RestoredFromCheckpoint");

		// Restore variables
		Score = SavedScore;
		SprintEnergy = SavedSprintEnergy;
		Hud?.UpdateSprint(SprintEnergy, true);
		Hud?.UpdateScore(Score);
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
		else if (inputEvent.IsActionPressed("Reload"))
		{
			if (OS.HasFeature("editor"))
			{
				SprintEnergy = 100;
			}
		}
	}

	
	public void ExtraSpeedBoost() {
		ExtraSpeedTimer = ExtraSpeedMaxTime*100;
		Hud!.ExtraSpeedBar!.MaxValue = ExtraSpeedTimer;
		Hud!.ExtraSpeedBar!.Visible = true;
	}

	void UpdateSpeed(float num) 
	{
		MoveSpeed *= num;
		AirMoveSpeed *= num;
		FlyMoveSpeed *= num;
	}
}
