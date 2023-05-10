using Godot;
using System;

public partial class SliderSetting : Panel, ISettingsItem
{
	[Export] Slider? Slider;

	public Variant GetValue()
	{
		return Slider?.Value ?? 14;
	}
}
