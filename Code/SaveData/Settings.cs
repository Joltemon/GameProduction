using System;

public static class Settings
{
    public static event Action? SettingsChanged;

    public static void UpdateSettings()
    {
        SettingsChanged?.Invoke();
    }
}