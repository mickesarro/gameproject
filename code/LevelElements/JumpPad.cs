using Sandbox;
using Shooter.Sounds;

namespace Shooter;

/// <summary>
/// Simple bounce pad that applies the specified amount of force to pad up direction
/// </summary>
public sealed class JumpPad : Component, Component.ITriggerListener
{
	[Property] private float Force { get; set; } = 100f;
    private Vector3 JumpForce = Vector3.Zero;

    [Property, ToggleGroup( "Trampoline", Label = "Trampoline" )]
    private bool Trampoline { get; set; } = false;
    [Property, ToggleGroup( "Trampoline" )] private float Factor { get; set; } = 1.5f;

    protected override void OnAwake()
    {
        base.OnAwake();

        JumpForce = WorldTransform.Up * Force;
    }


    public void OnTriggerEnter( Collider other )
	{
		if ( other.IsProxy ) return;

        if ( !other.GameObject.Root.Components.TryGet<ICharacterBase>( out var character ) ) return;

        var forceToApply = JumpForce;

        float velToPad = WorldTransform.Up.Dot( character.Velocity );
        // Only cancel motion if moving downwards
        if ( velToPad < 0 )
        {
            forceToApply += !Trampoline
                ? -WorldTransform.Up * velToPad
                : WorldTransform.Up * (-velToPad * Factor);
        }

        character.ApplyForce( forceToApply );

        // Makes sure that bots hitting the JumpPad don't play the sound to you locally
        if ( character.IsPlayer )
		{
			SoundManager.PlayLocal( SoundManager.SoundType.JumpPad, 0.5f );
		}
	}

}
