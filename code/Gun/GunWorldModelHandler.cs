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
	/// Set anchor object, remember to add the Anchor component to Parent of Parent.
	/// this class could be made more general purpose, atm just assumes hierarchy (asc):
	/// worldmodel -> gun prefab -> player prefab. Lisää muistiinpanoja koodissa
	/// </summary>

	// Anchor komponentti mahdollistaa vaan yhden objektin ankkuriksi, tätä luokkaa kannattaa käyttää
	// varmaan jatkossa myös itemien yms. piirtämiseenm, jolloin pitäisi erotella varmaankin tyypin mukaan
	// mikä ankkuri on aseille, itemeille tms.. Eri asetyypitkin voi vaatia eri ankkurin
	
	[Property] private String AnchorObjectName { get; set; }
	[Property] private Vector3 PositionOffset { get; set; } = Vector3.Zero;
	private GameObject anchor;
	
	protected override void OnAwake()
	{
		base.OnAwake();
		if ( Components.TryGet<SkinnedModelRenderer>( out var renderer ) 
			&& GameObject.Parent.Parent.Components.TryGet<Anchor>(out var anchorC) )
		{
			renderer.RenderType =
				Network.IsProxy ? ModelRenderer.ShadowRenderType.On : ModelRenderer.ShadowRenderType.ShadowsOnly;
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
		GameObject.WorldPosition = anchor.WorldPosition + worldOffset;
		GameObject.WorldRotation = anchor.WorldRotation;
	}
}
