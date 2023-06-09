using System;
using Godot;

interface ISettingsItem
{
	public string GetName();
	public Variant GetValue();
	public void SetValue(object? value);
}