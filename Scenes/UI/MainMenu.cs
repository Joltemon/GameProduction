using Godot;
using System;

public partial class MainMenu : Control
{

	Button button;

	

	void MouseEntered(int buttonID) {


		if (buttonID == 1) {
			button = GetNode<Button>("Buttons/StartButton");
		}
		
		if (buttonID == 2) {
			button = GetNode<Button>("Buttons/SettingsButton");
		}
		
		if (buttonID == 3) {
			button = GetNode<Button>("Buttons/CreditsButton");
		}
		
		if (buttonID == 4) {
			button = GetNode<Button>("Buttons/QuitButton");
		}
		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(button, "position", (button.Position.X + 40), 1.0f);
	}
}
