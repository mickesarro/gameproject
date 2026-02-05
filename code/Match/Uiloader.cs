using Shooter;

namespace Shooter;

public sealed class Uiloader : Component
{
    protected override void OnAwake()
    {
        if ( IsProxy )
        {
            Log.Info( "I am proxy" );
            return;
        }  
        Log.Info( Scene.Name );
        var slo = new SceneLoadOptions
        {
            IsAdditive = true
        };
        slo.SetScene( "/scenes/playerScene.scene_c" );
        Scene.Load( slo );
        Log.Info( Scene.Name );
        
        //DestroyGameObject();
    }
}
