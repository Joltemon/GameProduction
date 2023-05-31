using Godot;
using Tomlyn;
using Tomlyn.Model;
using System.IO;
using System.Collections.Generic;

class Savedata
{
	static TomlTable SettingsData = new();
	
	public static void Load()
	{
		var settingsPath = ProjectSettings.GlobalizePath("user://settings.save");

		if (File.Exists(settingsPath))
		{
			var settingsString = File.ReadAllText(settingsPath);
			SettingsData = Toml.ToModel(settingsString);
		}
	}
	
	public static T Get<T>(string name, T defaultValue)
	{
		T result;

		if (SettingsData?.ContainsKey(name) ?? false)
		{
			result = (T)SettingsData[name];
		}
		else
		{
			result = defaultValue;
		}

		return result;
	}

	public static object? Get(string name, object? defaultValue = null)
	{
		object? result;

		if (SettingsData?.ContainsKey(name) ?? false)
		{
			result = SettingsData[name];
		}
		else
		{
			result = defaultValue;
		}

		return result;
	}

	public static void Set(string name, Variant value, bool update = false)
	{
		if (value.Obj == null) return;

		if (SettingsData?.ContainsKey(name) ?? false)
		{
			SettingsData[name] = value.Obj;
		}
		else
		{
			SettingsData?.Add(name, value.Obj);
		}

		if (update) Save();
	}

	public static void Save()
	{
		if (SettingsData == null) return;

		var settingsPath = ProjectSettings.GlobalizePath("user://settings.save");
		var settingsString = Toml.FromModel(SettingsData);
		File.WriteAllText(settingsPath, settingsString);
	}
}