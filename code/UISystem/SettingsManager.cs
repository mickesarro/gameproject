using Sandbox;
using System;

public class SettingsManager : SingletonBase<SettingsManager>
{
	private PlayerPreferences playerPreferences = new();
	public PlayerPreferences PlayerPreferences => playerPreferences;

	public bool IsLoaded { get; private set; } = false;

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
    }

    public void SetCrosshairColor(string hex)
    {
        if (string.IsNullOrEmpty(hex) || playerPreferences.CrosshairColor == hex) return;

		playerPreferences.CrosshairColor = hex;
        OnCrosshairColorChanged?.Invoke(hex);
    }

    public void Save() // Called in SettingsMenu hide
    {
		FileManager.Save<PlayerPreferences>( playerPreferences );
    }

    public void Load()
    {
		FileManager.Load<PlayerPreferences>( ref playerPreferences );

        OnCrosshairStyleChanged?.Invoke( playerPreferences.CrosshairStyle );
        OnCrosshairColorChanged?.Invoke( playerPreferences.CrosshairColor );

		IsLoaded = true;
    }

}
