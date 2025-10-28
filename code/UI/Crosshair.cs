using Sandbox;
using Sandbox.Rendering;
using System;
using System.Linq;

public sealed class Crosshair : Component, IPlayerEvent
{
    private GunData gunData;
    private PlayerInventory inventory;

    public Color CurrentCrosshairColor { get; private set; } = Color.White;

    // --- Available colors ---
    public static readonly (string Name, string Hex)[] AvailableColors = new[]
    {
        ("White", "#FFFFFF"),
        ("Green", "#00FF00"),
        ("Cyan", "#00FFFF"),
        ("Yellow", "#FFFF00"),
        ("Red", "#FF0000"),
        ("Magenta", "#FF00FF")
    };

    protected override void OnAwake()
    {
        base.OnAwake();
        UpdateCrosshairColor(SettingsManager.CrosshairColor);
        SettingsManager.OnCrosshairColorChanged += UpdateCrosshairColor;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SettingsManager.OnCrosshairColorChanged -= UpdateCrosshairColor;
    }

    private void UpdateCrosshairColor(string hex)
    {
        try { CurrentCrosshairColor = Color.Parse(hex) ?? Color.White; }
        catch { CurrentCrosshairColor = Color.White; }
    }

    protected override void OnUpdate()
    {
        if (inventory?.CurrentWeapon == null || Scene.Camera == null) return;

        var hud = Scene.Camera.Hud;
        var center = Screen.Size * 0.5f;

        var crosshairType = SettingsManager.CrosshairStyle;

        switch (crosshairType)
        {
            case CrosshairType.Dot:
                DrawDot(hud, center);
                break;
            case CrosshairType.Circle:
                DrawCircle(hud, center);
                break;
            default:
                DrawLines(hud, center, crosshairType);
                break;
        }
    }

    void IPlayerEvent.OnSpawn(GameObject player)
    {
        if (player.IsProxy) return;
        gunData = player.GetComponent<GunData>();
        inventory = player.GetComponent<PlayerInventory>();
    }

    private void DrawLines(HudPainter hud, Vector2 center, CrosshairType type)
    {
        float gap = type == CrosshairType.Cross ? 0f : 3f;
        float length = 15f;
        float width = 2f;
        var color = CurrentCrosshairColor;

        if (type != CrosshairType.TStyle)
            hud.DrawLine(center + Vector2.Up * (length + gap), center + Vector2.Up * gap, width, color);

        hud.DrawLine(center + Vector2.Left * (length + gap), center + Vector2.Left * gap, width, color);
        hud.DrawLine(center + Vector2.Right * (length + gap), center + Vector2.Right * gap, width, color);
        hud.DrawLine(center + Vector2.Down * (length + gap), center + Vector2.Down * gap, width, color);
    }

    private void DrawDot(HudPainter hud, Vector2 center) => hud.DrawCircle(center, 3f, CurrentCrosshairColor);

    private void DrawCircle(HudPainter hud, Vector2 center)
    {
        DrawDot(hud, center);
        float size = 30f, width = 2f;
        hud.DrawRect(new Rect(center - new Vector2(size / 2), new Vector2(size, size)),
                     Color.Transparent,
                     new Vector4(all: size / 2),
                     new Vector4(all: width),
                     CurrentCrosshairColor);
    }
}

// --- Crosshair type enum ---
public enum CrosshairType
{
    Standard, // Four lines
    TStyle,   // Three lines (no top)
    Dot,      // Center dot
    Circle,   // Circle + dot
    Cross     // Four lines, no gap
}

// --- Enum display names ---
public static class CrosshairTypeExtensions
{
    public static string ToDisplayName(this CrosshairType type) =>
        type switch
        {
            CrosshairType.TStyle => "T-Style",
            _ => type.ToString()
        };
}
