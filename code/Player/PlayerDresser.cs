
namespace Shooter;

/// <summary>
/// Applies clothing for players characters.
/// </summary>
public sealed class PlayerClother : Component
{
	[Property] public SkinnedModelRenderer BodyRenderer { get; set; }

    protected override void OnStart()
	{
        // This is currently here, which means it run on respawn as well
        // That is probably not desirable, so search for alternative ways
        base.OnStart();

		var clothing = new ClothingContainer();
		clothing.Deserialize( GameObject.Network.Owner.GetUserData( "avatar" ) );
        clothing.Height = BodyRenderer.GetComponentInParent<PlayerController>().Height;
        
		clothing.Apply( BodyRenderer );

        // If there is a better way, please update
        if ( !Network.IsProxy )
        {
            // Loop over added clothing items to make them invisible for the owner
            foreach ( var c in BodyRenderer.GetComponentsInChildren<SkinnedModelRenderer>() )
            {
                c.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
            }
        }

        

    }
}
