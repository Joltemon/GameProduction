using Godot;
using System;

public partial class CheckBoxSetting : Panel, ISettingsItem
{
	[Export] CheckBox? CheckBox;

	public Variant GetValue()
	{
		return CheckBox?.ButtonPressed ?? false;
	}
}
