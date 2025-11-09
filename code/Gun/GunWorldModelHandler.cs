using System;
using Sandbox.Citizen;
using Sandbox.Helpers;

namespace Sandbox;

/// <summary>
/// Handles gun worldmodel.
/// </summary>
public sealed class GunWorldModelHandler : Component
{
	/// <summary>
	/// Set anchor object
	/// </summary>

	// Anchor komponentti mahdollistaa vaan yhden objektin ankkuriksi, tätä luokkaa kannattaa käyttää
	// varmaan jatkossa myös itemien yms. piirtämiseenm, jolloin pitäisi erotella varmaankin tyypin mukaan
	// mikä ankkuri on aseille, itemeille tms.. Eri asetyypitkin voi vaatia eri ankkurin
	
	[Property, Sync] private Vector3 PositionOffset { get; set; } = Vector3.Zero;
	private GameObject anchor;
	
	protected override void OnAwake()
	{
		base.OnAwake();
		if ( Components.TryGet<SkinnedModelRenderer>( out var renderer )) 
		{
			renderer.RenderType =
				Network.IsProxy ? ModelRenderer.ShadowRenderType.On : ModelRenderer.ShadowRenderType.ShadowsOnly;
			var anchorC = GameObject.GetComponentInParent<Anchor>();
			anchor = anchorC.Object;
		}
		else
		{
			Destroy();
		}
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		var worldOffset = anchor.WorldTransform.Rotation * PositionOffset;
		GameObject.WorldPosition = anchor.WorldPosition.WithZ( anchor.WorldPosition.z - 5f ) + worldOffset;
		GameObject.WorldRotation = anchor.WorldRotation;
	}
}
