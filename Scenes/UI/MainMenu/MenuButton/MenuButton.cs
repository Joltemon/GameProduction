using Godot;

public partial class MenuButton : Control
{
	[Signal] public delegate void PressedEventHandler();
	[Export] string Text = "";

	[Export] Vector2 HoverOffset;

	[ExportGroup("Components")]
	[Export] Button? Button;
	[Export] ColorRect? BlackOverlay;

	bool MouseOver;
	bool HasBeenPressed;

	public override void _Ready()
	{
		if (Button != null) Button.Text = Text;
	}
	
	public override void _Process(double delta)
	{
		if (Button == null || HasBeenPressed) return;
		
		if (MouseOver)
			Button.Position = Button.Position.Lerp(HoverOffset, 0.2f);
		else
			Button.Position = Button.Position.Lerp(Vector2.Zero, 0.2f);
	}

	void OnMouseEnter() => MouseOver = true;
	void OnMouseExit() => MouseOver = false;

	async void OnPressed()
	{
		HasBeenPressed = true;
		var windowSize = DisplayServer.WindowGetSize();
		var positionOffset = Size / 2;

		var finalScale = windowSize / positionOffset;

		var tween = GetTree().CreateTween().SetParallel().SetTrans(Tween.TransitionType.Cubic);
		
		tween.TweenProperty(this, "position", windowSize / 2 - positionOffset, 0.5f);
		tween.TweenProperty(this, "scale", finalScale, 0.5f).SetEase(Tween.EaseType.In);
		
		if (BlackOverlay != null)
			tween.TweenProperty(BlackOverlay, "color", new Color(0, 0, 0, 1), 0.5f);
		
		await ToSignal(tween, "finished");
		EmitSignal("Pressed");
	}
}
