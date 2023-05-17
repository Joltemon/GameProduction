using Godot;
using System;

public partial class CheckBoxSetting : Panel, ISettingsItem
{
	[Export] CheckButton? CheckBox;

	public string GetName() => Name;

	public Variant GetValue()
	{
		return CheckBox?.ButtonPressed ?? false;
	}

	public void SetValue(object? value)
	{
		if (CheckBox != null && value != null)
			CheckBox.ButtonPressed = (bool)value;
	}
}
