using Sandbox;

namespace Shooter;

/// <summary>
/// Common interface to enable players and other characters to react similarly
/// </summary>
public interface ICharacterBase
{
    public bool IsPlayer { get; }
	public PlayerStats CharacterStats { get; }
	public void ApplyForce( Vector3 amount );
}
