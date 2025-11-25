using Sandbox;

namespace Shooter;

/// <summary>
/// Common interface to enable players and other characters to react similarly
/// </summary>
public interface ICharacterBase
{
	public PlayerStats CharacterStats { get; }
	public void ApplyForce( Vector3 amount );

    public void ShakeScreen( ScreenShake screenShake ) { }
}
