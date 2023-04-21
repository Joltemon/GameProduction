using Godot;
using System;

public partial class Dialog : Control
{
	// Annoying hard-coded file path, can't seem to be able to get the current scenes path without creating an instance of it
	static NodePath DialogScenePath = "res://Scenes/UI/Dialog/Dialog.tscn";
	
	public override void _Ready()
	{
	}

	public void ShowMessage()
	{
		
	}

	public void ShowConfirm()
	{

	}
	
	public static Dialog NewDialog()
	{
		return GD.Load<PackedScene>(DialogScenePath).Instantiate<Dialog>();
	}
}
