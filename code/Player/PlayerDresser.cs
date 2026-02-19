
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

    private ClothingContainer clothing;

    public void SaveClothing()
    {
        clothing = ClothingContainer.CreateFromLocalUser();
    }

    public float tint { get; }

    // korjaa casen kun pelaaja liittyy kesken toisen spawnin ja vaatteet ei mee päälle
    [Rpc.Broadcast]
    public void ApplyClothing()
    {
        _ = ApplyClothingInternal();
    }

    private async Task ApplyClothingInternal()
    {
        await AsyncDresser.Instance.Add(bodyRenderer, false, clothing);

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

    protected override void OnStart()
    {
        // This is currently here, which means it run on respawn as well
        // That is probably not desirable, so search for alternative ways
        base.OnStart();
        SaveClothing();
        // ApplyClothing();
    }
}
