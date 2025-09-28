
using System;

/// <summary>
/// Component to enable shooting for guns
/// </summary>
public sealed class Shoot : Component
{

	[Property] private float loadTime = 1.0f;

	[Property] private GameObject projectilePrefab { get; set; }
	[Property] private GameObject barrelEnd; // Spawn point
	[Property] private GameObject viewmodel { get; set; }

	protected override void OnAwake()
	{
		if (projectilePrefab == null || barrelEnd == null)
		{
			Log.Warning( "Properties not set!" );
			DestroyGameObject();
		}
	}

	private float elapsed = 0.0f;
	protected override void OnUpdate()
	{
		// Should be implemented with some form of events perhaps
		if ( Input.Pressed( "attack1" ) && elapsed > loadTime )
		{
			// tää ei tunnu spawnaavan oikeesti piipun päästä, ei oo kriittistä kyllä vielä
			// + pitääköhän projektiilin spawnata ruudun keskeltä ja näyttää client side vaan jotai muuta?
			var projectile = projectilePrefab.Clone( barrelEnd.WorldTransform );
			elapsed = 0.0f;
			Animations();

		}

		elapsed += Time.Delta;
	}

	private void Animations()
	{
		var modelRenderer = viewmodel.Components.Get<SkinnedModelRenderer>();
		modelRenderer.Parameters.Set( "fire", true );
	}
}
