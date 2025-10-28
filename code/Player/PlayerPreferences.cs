using System;
using Sandbox;

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
	private const float MaxVolume = 100f;
	private const float MinVolume = 0f;

	private float volume = 50f;
	public float Volume 
	{
		get => volume;
		set
		{
			volume = Math.Clamp(value, MinVolume, MaxVolume);
		}
	}

	private const float minFov = 60f;
	private const float maxFov = 140f;

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

}
