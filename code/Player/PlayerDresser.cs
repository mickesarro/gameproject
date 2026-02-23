
using System.Threading.Tasks;
using Shooter.Helpers;

namespace Shooter;

/// <summary>
/// Applies clothing for players characters.
/// </summary>
public sealed class PlayerDresser : Component, ICharacterDresser
{
    [Property] private SkinnedModelRenderer bodyRenderer;
    public SkinnedModelRenderer BodyRenderer => bodyRenderer;

    private ClothingContainer clothing = null;

    // Handles checking and retrying setting clothes if container not yet loaded
    private enum ClothStatus { Unset, Tried, Loaded };
    private ClothStatus clothStatus = ClothStatus.Unset;

    protected override void OnStart()
    {
        base.OnStart();

        var owner = Network.Owner;

        if ( owner == null ) return;

        clothing = new();
        clothing.Deserialize( owner.GetUserData( "avatar" ) );

        var oldstatus = clothStatus;
        clothStatus = ClothStatus.Loaded;

        if ( oldstatus == ClothStatus.Tried )
        {
            ApplyClothing();
        }
    }

    public void SaveClothing()
    {
        clothing = ClothingContainer.CreateFromLocalUser();
    }

    public float tint { get; }

    // korjaa casen kun pelaaja liittyy kesken toisen spawnin ja vaatteet ei mee päälle
    //[Rpc.Broadcast]
    public void ApplyClothing()
    {
        if ( clothStatus != ClothStatus.Loaded )
        {
            clothStatus = ClothStatus.Tried;
            return;
        }

        _ = ApplyClothingInternal();
    }

    private async Task ApplyClothingInternal()
    {
        await AsyncDresser.Instance.Add( bodyRenderer, false, clothing );

        if (!Network.IsProxy)
        {
            foreach (var c in bodyRenderer.GetComponentsInChildren<SkinnedModelRenderer>())
            {
                c.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
            }
        }
    }
    
    public void ClearClothing()
    {
        clothing.Clothing.Clear();
    }
}
