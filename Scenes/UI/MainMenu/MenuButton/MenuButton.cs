using Godot;

public partial class MenuButton : Button
{
	[Signal] public delegate void PressedEventHandler();
	Vector2 BasePosition;
	bool HasBeenPressed;

	public override void _Ready()
	{
		BasePosition = Position;
	}

	void OnMouseEnter()
	{
		if (!HasBeenPressed)
			GetTree().CreateTween().TweenProperty(this, "position", new Vector2(Position.X + 20, Position.Y), 0.1f);
	}
	void OnMouseExit()
	{
		if (!HasBeenPressed)
			GetTree().CreateTween().TweenProperty(this, "position", new Vector2(Position.X - 20, Position.Y), 0.2f);
	}

	async void OnPressed()
	{
		HasBeenPressed = true;
		var windowSize = DisplayServer.WindowGetSize();
		var positionOffset = Size / 2;

		var finalScale = windowSize / Size;

		var tween = GetTree().CreateTween().SetParallel().SetTrans(Tween.TransitionType.Cubic);
		tween.TweenProperty(this, "position", windowSize / 2 - positionOffset, 0.5f);
		tween.TweenProperty(this, "scale", finalScale, 0.5f).SetEase(Tween.EaseType.In);
		tween.TweenProperty(this, "modulate", new Color(0, 0, 0), 0.5f);
		
		await ToSignal(tween, "finished");
		EmitSignal("Pressed");
	}
}
