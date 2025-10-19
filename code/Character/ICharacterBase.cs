using Sandbox;

/// <summary>
/// Common interface to enable players and other characters to react similarly
/// </summary>
public interface ICharacterBase
{
	public PlayerStats CharacterStats { get; }
	public void Punch( Vector3 amount );

}
