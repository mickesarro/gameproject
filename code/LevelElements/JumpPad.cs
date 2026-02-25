using Sandbox;
using Shooter.Sounds;

namespace Shooter;

/// <summary>
/// Simple bounce pad that applies the specified amount of force to pad up direction
/// </summary>
public sealed class JumpPad : Component, Component.ITriggerListener
{
	[Property] private float Force { get; set; } = 100f;

	public void OnTriggerEnter( Collider other )
	{
		if ( other.IsProxy ) return;

		other.GameObject.Root.GetComponent<ICharacterBase>()
			?.ApplyForce( WorldTransform.Up * Force );

		// Makes sure that bots hitting the JumpPad don't play the sound to you locally
		if ( other.GameObject.Root.Components.TryGet<PlayerController>( out _ ) )
		{
			SoundManager.PlayLocal( SoundManager.SoundType.JumpPad, 0.5f );
		}
	}

}
