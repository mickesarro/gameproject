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

    public async void ApplyClothing()
    {
        var task = AsyncDresser.Instance.Add( bodyRenderer );
    }
    
    protected override void OnAwake()
    {
        if ( !ShouldDress )
        {
            DestroyGameObject();
            return;
        }
        base.OnAwake();
        ApplyClothing();
    }
}
