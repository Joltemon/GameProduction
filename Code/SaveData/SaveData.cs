using Godot;
using Tomlyn;
using System.IO;
using System.Collections.Generic;

class Savedata
{
	static Dictionary<string, Variant> SettingsData = new();
	
	public static void Load()
	{
		var settingsPath = ProjectSettings.GlobalizePath("user://settings.save");
		var settingsString = File.ReadAllText(settingsPath);
		SettingsData = Toml.ToModel<Dictionary<string, Variant>>(settingsString);
	}
	
	public static T Get<[MustBeVariant] T>(string name, T defaultValue)
	{
		T result;

		if (SettingsData.ContainsKey(name))
		{
			result = SettingsData[name].As<T>();
		}
		else
		{
			result = defaultValue;
		}
		return result;
	}

	public static void Set(string name, Variant value, bool update = false)
	{
		if (SettingsData.ContainsKey(name))
		{
			SettingsData[name] = value;
		}
		else
		{
			SettingsData.Add(name, value);
		}

		if (update) Save();
	}

	public static void Save()
	{
		var settingsPath = ProjectSettings.GlobalizePath("user://settings.save");
		var settingsString = Toml.FromModel(SettingsData);
		File.WriteAllText(settingsPath, settingsString);
	}
}