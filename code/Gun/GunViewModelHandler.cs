using System;

namespace Shooter;

/// <summary>
/// Handles the movement of the gun.
/// </summary>
public class GunViewModelHandler : Component
{
    private CameraComponent camera;
    protected override void OnAwake()
    {
        if (IsProxy) return;
        base.OnAwake();

        if (Scene.Camera == null )
        {
            Log.Info( "No camera found, destroying." );
            Destroy();
        }
        
        // NPC does not need viewmodel
        if ( Tags.Has( "npc" ))
        {
            var arms = GameObject.Parent?.Children?.Find(o => o.Name == "arms");
            var viewmodel = GameObject.Parent?.Children?.Find(o => o.Name == "viewmodel");
            arms?.Destroy();
            viewmodel?.Destroy();
            Destroy();
        }
    }

    protected override void OnEnabled()
    {
        base.OnEnabled();

        camera = Scene.Camera;

        if (camera == null)
        {
            camera = new();
        }
    }
    
    protected override void OnPreRender()
    {
        if (IsProxy) return;
        base.OnPreRender();

        // This is not ideal and must be made independent later.
        GameObject.WorldPosition = camera.WorldPosition;
        GameObject.WorldRotation = camera.WorldRotation;
    }
}
