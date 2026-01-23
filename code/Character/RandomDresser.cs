using Sandbox;

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

    [Property] private Dresser dresser;
    [Property] private bool ShouldDress = false;

    public async void ApplyClothing()
    {
        dresser.Randomize();

        //await dresser.Apply();
    }

    protected override void OnAwake()
    {
        if ( !ShouldDress )
        {
            DestroyGameObject();
            return;
        }

        base.OnAwake();

        dresser ??= Components.Create<Dresser>();
        dresser.BodyTarget = bodyRenderer;
    }

    protected override void OnStart()
    {
        base.OnStart();

        ApplyClothing();
    }
}
