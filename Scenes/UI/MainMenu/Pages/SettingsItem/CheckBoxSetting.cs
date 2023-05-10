using Godot;
using System;

public partial class CheckBoxSetting : Panel, ISettingsItem
{
	[Export] CheckBox? CheckBox;

	public string GetName() => Name;

	public Variant GetValue()
	{
		return CheckBox?.ButtonPressed ?? false;
	}

	public void SetValue(Variant result)
	{
		if (CheckBox != null)
			CheckBox.ButtonPressed = result.As<bool>();
	}
}
