using Sandbox;

public sealed class PlayerPreferences : ISerializable
{
	public string Name => "player_preferences";
	public bool ShouldAccumulate => false;

	public float Volume { get; set; }
	// etc.

}
