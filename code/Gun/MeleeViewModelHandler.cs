using System;

namespace Shooter;

public sealed class MeleeViewModelHandler : GunViewModelHandler
{
    protected override void OnAwake()
    {
        if ( IsProxy ) return;
        base.OnAwake();

        if ( Scene.Camera == null )
        {
            Log.Info( "No camera found, destroying." );
            Destroy();
        }

        // NPC does not need viewmodel
        if ( Tags.Has( "npc" ) )
        {
            GameObject.Parent?.Children?.Find( o => o.Name == "viewmodel" )
                ?.Destroy();

            Destroy();
        }
    }
}
