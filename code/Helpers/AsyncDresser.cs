using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Shooter.Helpers;

public class AsyncDresser: SingletonBase<AsyncDresser>, ISceneLoadingEvents
{
    [Property, RequireComponent] public static Dresser dresser {get; set;}
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    Task ISceneLoadingEvents.OnLoad( Scene scene, SceneLoadOptions options )
    {
        return Task.CompletedTask;
    }

    public async ValueTask Add( SkinnedModelRenderer bodyRenderer, bool random, ClothingContainer? clothingContainer)
    {
        await _semaphore.WaitAsync();
        try
        {
            await Task.Delay( 10 );
            
            if ( random )
            {
                await RandomizeAndApply( bodyRenderer );
            }
            else
            {
                await Apply( bodyRenderer, clothingContainer );
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private Random randomizer = new();

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
        //dresser.ManualHeight = randomizer.Float();
        dresser.ManualTint = randomizer.Float();
        
        await dresser.Apply();
    }

    public async ValueTask Apply( SkinnedModelRenderer bodyRenderer, ClothingContainer clothing )
    {
        dresser.BodyTarget = bodyRenderer;
        dresser.Clothing = clothing.Clothing;
        await dresser.Apply();
    }

    public async ValueTask ApplyRandom( SkinnedModelRenderer bodyRenderer, ClothingContainer clothing, float tint, float age )
    {
        dresser.BodyTarget = bodyRenderer;
        var random = clothing.Clothing;
        
        dresser.Clothing = [];
        
        dresser.Clothing.Clear();
        dresser.Clothing.AddRange( random );

        dresser.ManualAge = age;
        //dresser.ManualHeight = randomizer.Float();
        dresser.ManualTint = tint;
        
        await dresser.Apply();
    }

    public List<ClothingContainer.ClothingEntry> Randomize()
    {
        var random = AvatarRandomizer.GetRandom();
        return random.ToList();
    }
    
    public void RestoreToDefault( SkinnedModelRenderer bodyRenderer )
    {
        dresser.BodyTarget = bodyRenderer;
        dresser.Clothing.Clear();
    }
}
