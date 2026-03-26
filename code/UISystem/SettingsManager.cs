using Sandbox;
using Sandbox.Audio;
using System;
using Shooter.UI;

namespace Shooter;
public class SettingsManager : SingletonBase<SettingsManager>
{
	private PlayerPreferences playerPreferences = new();
	public PlayerPreferences PlayerPreferences => playerPreferences;

	public bool IsLoaded { get; private set; } = false;
	// !!NOTE: Remember to add to all new setting setters or come up with new pattern
	private bool stateChanged = false; // To avoid unnecessary writes

	public event Action<CrosshairType> OnCrosshairStyleChanged;
    public event Action<string> OnCrosshairColorChanged;
	public event Action<string> OnHudColorChanged;
	public event Action<float> OnFovChanged;
	public event Action<float> OnVolumeChanged;
	public event Action<SpeedDisplayPosition> OnSpeedDisplayChanged;

	private readonly Dictionary<CameraComponent, Action<float>> cameraFovHandlers = new();

	protected override void OnAwake()
	{
		base.OnAwake();

		Load();

		ApplyVolume(playerPreferences.Volume);
	}

    public void SetCrosshairStyle(CrosshairType type)
    {
        if ( playerPreferences.CrosshairStyle == type) return;

		playerPreferences.CrosshairStyle = type;
        OnCrosshairStyleChanged?.Invoke(type);

		stateChanged = true;
	}

    public void SetCrosshairColor(string hex)
    {
        if (string.IsNullOrEmpty(hex) || playerPreferences.CrosshairColor == hex) return;

		playerPreferences.CrosshairColor = hex;
        OnCrosshairColorChanged?.Invoke(hex);

		stateChanged = true;
	}

	public void SetHudColor(string hex)
    {
        if (string.IsNullOrEmpty(hex) || playerPreferences.HudColor == hex) return;

        playerPreferences.HudColor = hex;
        OnHudColorChanged?.Invoke(hex);

        stateChanged = true;
    }

	public void SetSpeedDisplay(SpeedDisplayPosition position)
	{
		if (playerPreferences.SpeedDisplay == position) return;
		
        playerPreferences.SpeedDisplay = position;
		OnSpeedDisplayChanged?.Invoke(position);
		
        stateChanged = true;
	}

	public void SetTutorialComplete(bool complete)
    {
        if (playerPreferences.TutorialComplete == complete) return;

        playerPreferences.TutorialComplete = complete;
        stateChanged = true;
        Save();
    }

	public void SetFOV(float fov)
	{
		playerPreferences.Fov = fov; 
		OnFovChanged?.Invoke(fov);
		stateChanged = true;
	}


	public void SubscribeCameraFOV(CameraComponent camera, bool useCustomFOV)
	{
		if (camera == null) 
			return;

		// Remove the old handler if this camera was already subscribed.
		// For example, if the player respawns, there would otherwise be two handlers.
		if (cameraFovHandlers.ContainsKey(camera))
		{
			OnFovChanged -= cameraFovHandlers[camera];
			cameraFovHandlers.Remove(camera);
		}

		// Create a new handler
		cameraFovHandlers[camera] = fov =>
		{
			Log.Info("Changing Fov");
			camera.FieldOfView = fov;
		};

		OnFovChanged += cameraFovHandlers[camera];

		camera.FieldOfView = PlayerPreferences.Fov;

	}
	public void SetVolume(float volume)
	{
		playerPreferences.Volume = volume;

		ApplyVolume(playerPreferences.Volume);

		OnVolumeChanged?.Invoke(volume);
		stateChanged = true;
	}

	private void ApplyVolume(float volume)
	{
		Mixer.Master.Volume = volume / 100f;
	}

    public void Save() // Called in SettingsMenu hide
    {
		if ( !IsLoaded || !stateChanged ) return;

		FileManager.Save<PlayerPreferences>( playerPreferences );
		stateChanged = false;
    }

    public void Load()
    {
		FileManager.Load<PlayerPreferences>( ref playerPreferences );

        OnCrosshairStyleChanged?.Invoke( playerPreferences.CrosshairStyle );
        OnCrosshairColorChanged?.Invoke( playerPreferences.CrosshairColor );
        OnHudColorChanged?.Invoke( playerPreferences.HudColor );
        OnFovChanged?.Invoke( playerPreferences.Fov );
        OnVolumeChanged?.Invoke( playerPreferences.Volume );
		OnSpeedDisplayChanged?.Invoke( playerPreferences.SpeedDisplay );

		IsLoaded = true;
    }

}
