using Sandbox;
using System;
using System.Collections.Generic;

namespace Shooter;

public sealed class ScreenShaker : Component
{
    private CameraComponent camera;

    // Could use a list
    private readonly List<ScreenShake> screenShakes = new();

    protected override void OnEnabled()
    {
        base.OnEnabled();

        camera = Scene.Camera;
    }

    public void QueueScreenShake( ScreenShake screenShake )
    {
        screenShakes.Add( screenShake );
        screenShake.TimeSince = 0;
    }

    private Random random = new();
    public void UpdateShake()
    {
        if ( screenShakes.Count == 0 ) return;

        float shakePos = 0f;
        Rotation shakeRot = camera.LocalRotation;
        for ( int i = 0; i < screenShakes.Count; ++i )
        {
            var screenShake = screenShakes[i];
            if ( screenShake.TimeSince < screenShake.Duration )
            {
                shakePos += random.Float( 0, screenShake.Magnitude );
                shakeRot *= screenShake.Rotation;
            }
            else
            {
                screenShakes.RemoveAt( i );
            }
        }
        camera.LocalPosition += shakePos / screenShakes.Count;

        camera.LocalRotation = shakeRot;
    }

}
