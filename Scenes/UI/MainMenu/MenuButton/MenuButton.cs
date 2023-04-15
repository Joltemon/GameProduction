using Godot;

public partial class MenuButton : Control
{
	Vector2 BasePosition;

	public override void _Ready()
	{
		BasePosition = Position;
	}

	void OnMouseEnter() => GetTree().CreateTween().TweenProperty(this, "position", new Vector2(BasePosition.X + 20, BasePosition.Y), 0.1f);
	void OnMouseExit() => GetTree().CreateTween().TweenProperty(this, "position", BasePosition, 0.2f);
}
