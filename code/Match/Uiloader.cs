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
        var slo = new SceneLoadOptions();
        slo.IsAdditive = true;
        slo.SetScene( "/scenes/playerScene.scene" );
        Scene.Load( slo );
        Log.Info( Scene.Name );
        
        //DestroyGameObject();
    }
}
