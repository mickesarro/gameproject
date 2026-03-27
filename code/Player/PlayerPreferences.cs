using System;
using Shooter.UI;

namespace Shooter;

public enum SpeedDisplayPosition
{
    Off,
    UnderCrosshair,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

/// <summary>
/// Used as a data class for the preferences of the player.
/// Can be serialized with FileManager without type conversions.
/// </summary>
public sealed class PlayerPreferences : ISerializable
{
	public string Name => "player_preferences";
	public bool ShouldAccumulate => false;

	// Settings constant ranges could be defined elsewhere or just omitted here
	// if we trust the srouces
	public readonly float maxVolume = 100f;
	public readonly float minVolume = 0f;

	private float volume = 50f;
	public float Volume 
	{
		get => volume;
		set
		{
			volume = Math.Clamp(value, minVolume, maxVolume);
		}
	}

	public readonly float minFov = 60f;
	public readonly float maxFov = 120f;

	private float fov = 90f;
	public float Fov
	{
		get => fov;
		set
		{
			fov = Math.Clamp( value, minFov, maxFov );
		}
	}

	public CrosshairType CrosshairStyle { get; set; } = CrosshairType.Standard;
	public string CrosshairColor { get; set; } = "#FFFFFF";
	public string HudColor { get; set; } = "#FFFFFF";
	public SpeedDisplayPosition SpeedDisplay { get; set; } = SpeedDisplayPosition.Off;
	public bool EasyStrafe { get; set; } = false;
	public bool TutorialComplete { get; set; } = false;

}
