using Godot;
using System;

public partial class SliderSetting : Panel, ISettingsItem
{
	[Export] Slider? Slider;

	public Variant GetValue()
	{
		return Slider?.Value ?? 0;
	}

	public string GetName() => Name;

	public void SetValue(object? value)
	{
		if (Slider != null && value != null)
			Slider.Value = (double)value;
	}
}
