using Sandbox;

public sealed class PlayerPreferences : ISerializable
{
	public string Name => "player_preferences";

	public float Volume { get; set; }
	// etc.

}
