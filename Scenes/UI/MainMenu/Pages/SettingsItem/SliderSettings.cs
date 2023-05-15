using Godot;
using System;

public partial class SliderSetting : Panel, ISettingsItem
{
	[Export] Slider? Slider;

	public Variant GetValue()
	{
		return Slider?.Value ?? 14;
	}

	public string GetName() => Name;

	public void SetValue(Variant value)
	{
		if (Slider != null)
			Slider.Value = value.As<float>();
	}
}
