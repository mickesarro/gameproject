using Sandbox;
using System;

public class SettingsManager : SingletonBase<SettingsManager>
{
	private PlayerPreferences playerPreferences = new();
	public PlayerPreferences PlayerPreferences => playerPreferences;

	public bool IsLoaded { get; private set; } = false;
	// !!NOTE: Remember to add to all new setting setters or come up with new pattern
	private bool stateChanged = false; // To avoid unnecessary writes

	public event Action<CrosshairType> OnCrosshairStyleChanged;
    public event Action<string> OnCrosshairColorChanged;

	protected override void OnAwake()
	{
		base.OnAwake();

		Load();
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

		IsLoaded = true;
    }

}
