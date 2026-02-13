using System;
using Shooter.Helpers;

namespace Shooter;

/// <summary>
/// Randomly assigns an outfit to the character.
/// </summary>
public sealed class RandomDresser : Component, ICharacterDresser
{
    // This should be made persistent i.e. not generate on respawn
    // Once on game start and reuse it

    [Property] private SkinnedModelRenderer bodyRenderer;
    public SkinnedModelRenderer BodyRenderer => bodyRenderer;

    [Property] private bool ShouldDress = false;
    
    private ClothingContainer clothing = new ClothingContainer();
    
    private Random randomizer = new Random();
    public float tint { get; private set; }
    public float age;

    public async void ApplyClothing()
    {
        AsyncDresser.Instance.ApplyRandom( bodyRenderer, clothing, tint, age );
    }

    public void SaveClothing()
    {
        tint = randomizer.Float().Clamp( 0f, 1f );
        age = randomizer.Float().Clamp( 0f, 1f );
        clothing.Clothing = AsyncDresser.Instance.Randomize();
    }

    public void ClearClothing()
    {
        clothing.Clothing.Clear();
    }

    protected override void OnAwake()
    {
        if ( !ShouldDress )
        {
            Destroy();
            return;
        }
        base.OnAwake();
        SaveClothing();
    }
}
