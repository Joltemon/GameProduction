using Godot;
using System;

public partial class Title : Control
{

	AnimationPlayer? TitleAnimation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TitleAnimation = GetNode<AnimationPlayer>("AnimationPlayer");
		TitleAnimation.Play("Startup");
	}
}

