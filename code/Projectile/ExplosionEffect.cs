using Sandbox;

namespace Shooter;

/// <summary>
/// Spawns a visual explosion effect at this object's position on start.
/// Designed to be paired with BlastEffect on the same prefab.
/// The visual is spawned locally on each client, so it does not need to be networked.
/// </summary>
public sealed class ExplosionEffect : Component
{
	[Property] public GameObject EffectPrefab { get; set; }

	/// <summary>
	/// Uniform scale multiplier applied to the spawned visual effect.
	/// </summary>
	[Property, Range( 0.1f, 5f )] public float Scale { get; set; } = 1.0f;

	protected override void OnStart()
	{
		if ( EffectPrefab is null )
		{
			Log.Warning( "ExplosionEffect: EffectPrefab not set" );
			return;
		}

		// Clone disabled so DamageOnEnabled components don't fire before we can remove them.
		// Damage is handled exclusively by BlastEffect.
		var effect = EffectPrefab.Clone( new CloneConfig( new Transform( WorldPosition ), startEnabled: false ) );
		effect.LocalScale = Vector3.One * Scale;
		effect.Components.Get<Sandbox.RadiusDamage>( includeDisabled: true )?.Destroy();
		effect.Enabled = true;
	}
}
