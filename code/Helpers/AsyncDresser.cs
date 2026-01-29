using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shooter.Helpers;

public class AsyncDresser: SingletonBase<AsyncDresser>, ISceneLoadingEvents
{
    [Property, RequireComponent] public static Dresser dresser {get; set;}
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    Task ISceneLoadingEvents.OnLoad( Scene scene, SceneLoadOptions options )
    {
        randomizer = new();
        return Task.CompletedTask;
    }

    public async ValueTask Add( SkinnedModelRenderer bodyRenderer )
    {
        await _semaphore.WaitAsync();
        try
        {
            await RandomizeAndApply(bodyRenderer);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private Random randomizer;

    public async ValueTask RandomizeAndApply(SkinnedModelRenderer bodyRenderer)
    {
        dresser.BodyTarget = bodyRenderer;
        var random = AvatarRandomizer.GetRandom();
        if ( !dresser.IsValid() || !dresser.BodyTarget.IsValid() )
            return;

        dresser.Clothing = [];
        
        dresser.Clothing.Clear();
        dresser.Clothing.AddRange( random );
        
        dresser.ManualAge = randomizer.Float();
        dresser.ManualHeight = randomizer.Float();
        dresser.ManualTint = randomizer.Float();
        
        await dresser.Apply();
    }
}
