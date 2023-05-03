using Godot;

public partial class MenuButton : Control
{
	[Signal] public delegate void PressedEventHandler();
	[Export] string Text = "";

	[Export] Vector2 HoverOffset;

	[ExportGroup("Components")]
	[Export] Button? Button;
	[Export] TextureRect? Overlay;
	[Export] Texture2D? OverlayTexture;

	bool MouseOver;
	bool HasBeenPressed;

	public override void _Ready()
	{
		if (Button != null) Button.Text = Text;
		if (Overlay != null) Overlay.Texture = OverlayTexture;
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
		if (Button != null)
			Button.Position = Vector2.Zero;
		var windowSize = DisplayServer.WindowGetSize();
		var positionOffset = Size/2;

		var finalScale = windowSize / Size; 

		var tween = GetTree().CreateTween().SetParallel().SetTrans(Tween.TransitionType.Cubic);

		Vector2 newPosition = (windowSize/2) - positionOffset; //new Vector2((windowSize.X / 2), Size.Y/2 - Size);
		
		tween.TweenProperty(this, "position", newPosition, 0.5f);
		tween.TweenProperty(this, "scale", finalScale, 0.5f).SetEase(Tween.EaseType.In);
		
		if (Overlay != null)
			tween.TweenProperty(Overlay, "modulate", new Color(1, 1, 1, 1), 0.5f);
		
		await ToSignal(tween, "finished");
		EmitSignal("Pressed");
	}
}
