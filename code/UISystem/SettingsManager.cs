using Sandbox;
using System;

public static class SettingsManager
{
    public static CrosshairType CrosshairStyle { get; private set; } = CrosshairType.Standard;
    public static string CrosshairColor { get; private set; } = "#FFFFFF";

    public static event Action<CrosshairType> OnCrosshairStyleChanged;
    public static event Action<string> OnCrosshairColorChanged;

    public static void SetCrosshairStyle(CrosshairType type)
    {
        if (CrosshairStyle == type) return;
        CrosshairStyle = type;
        OnCrosshairStyleChanged?.Invoke(type);
        Save();
    }

    public static void SetCrosshairColor(string hex)
    {
        if (string.IsNullOrEmpty(hex) || CrosshairColor == hex) return;
        CrosshairColor = hex;
        OnCrosshairColorChanged?.Invoke(hex);
        Save();
    }

    public static void Save()
    {
        var data = new { CrosshairStyle = CrosshairStyle.ToString(), CrosshairColor };
        FileSystem.Data.WriteJson("settings.json", data);
    }

    public static void Load()
    {
        var defaults = new { CrosshairStyle = CrosshairType.Standard.ToString(), CrosshairColor = "#FFFFFF" };
        var data = FileSystem.Data.ReadJson("settings.json", defaults);

        if (Enum.TryParse(data.CrosshairStyle, ignoreCase: true, out CrosshairType parsed))
            CrosshairStyle = parsed;

        CrosshairColor = data.CrosshairColor;

        OnCrosshairStyleChanged?.Invoke(CrosshairStyle);
        OnCrosshairColorChanged?.Invoke(CrosshairColor);
    }
}
