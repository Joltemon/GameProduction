using Godot;
using Tomlyn;
using System.IO;
using System.Collections.Generic;

class Savedata
{
	static Dictionary<string, Variant>? SettingsData = new();

	public static bool Loaded;
	
	public static void Load()
	{
		var settingsPath = ProjectSettings.GlobalizePath("user://settings.save");
		if (File.Exists(settingsPath))
		{
			var settingsString = File.ReadAllText(settingsPath);
			SettingsData = Toml.ToModel<Dictionary<string, Variant>>(settingsString);
		}
	}
	
	public static T Get<[MustBeVariant] T>(string name, T defaultValue)
	{
		T result;

		if (SettingsData?.ContainsKey(name) ?? false)
		{
			result = SettingsData[name].As<T>();
		}
		else
		{
			result = defaultValue;
		}
		return result;
	}

	public static Variant Get(string name, Variant defaultValue = new Variant())
	{
		Variant result;

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
		if (SettingsData?.ContainsKey(name) ?? false)
		{
			SettingsData[name] = value;
		}
		else
		{
			SettingsData?.Add(name, value);
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

// [JsonSerializable(typeof(Dictionary<string, Variant>))]
// partial class JsonContext : JsonSerializerContext {}