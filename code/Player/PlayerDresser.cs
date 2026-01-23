
namespace Shooter;

/// <summary>
/// Applies clothing for players characters.
/// </summary>
public sealed class PlayerDresser : Component, ICharacterDresser
{
    [Property] private SkinnedModelRenderer bodyRenderer;
    public SkinnedModelRenderer BodyRenderer => bodyRenderer;

    public void ApplyClothing()
	{
		var clothing = new ClothingContainer();
		clothing.Deserialize( GameObject.Network.Owner.GetUserData( "avatar" ) );
        clothing.Height = bodyRenderer.GetComponentInParent<PlayerController>().Height;
        
		clothing.Apply( bodyRenderer );

        // If there is a better way, please update
        if ( !Network.IsProxy )
        {
            // Loop over added clothing items to make them invisible for the owner
            foreach ( var c in bodyRenderer.GetComponentsInChildren<SkinnedModelRenderer>() )
            {
                c.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
            }
        }
    }

    protected override void OnStart()
    {
        // This is currently here, which means it run on respawn as well
        // That is probably not desirable, so search for alternative ways
        base.OnStart();

        ApplyClothing();
    }

}
