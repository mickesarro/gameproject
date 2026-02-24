using Sandbox;

namespace Shooter.Camera;

/// <summary>
/// Represents the death camera after dying.
/// Player can orbit the ragdoll with this component.
/// </summary>
public sealed class OrbitCamera : BoundedCamera
{
    // How far to orbit from
    [Property] public float Radius { get; set; } = 50.0f;
    private Vector3 vecRadius = Vector3.Zero;

    private TimeSince TimeSince;

    // Defines how long the camera remains
    [Property] public float Lifetime { get; private set; } = 10.0f;
    [Property] private float MinWaitTime = 1.0f;

    protected override void OnEnabled()
    {
        base.OnEnabled();

        TimeSince = 0.0f;
        skipPressed = 0.0f;
        vecRadius = new Vector3( Radius );
    }

    private TimeSince skipPressed;
    protected override void OnUpdate()
    {
        base.OnUpdate();

        if ( TimeSince > Lifetime )
        {
            Respawn();
        }

        if ( Input.Down("Skip") )
        {
            if ( skipPressed > MinWaitTime ) {
                Respawn();
            }
            return;
        }
        
        skipPressed = 0.0f;  
    }

    protected override void OnDisabled()
    {
        base.OnDisabled();

        // This is so it doesn't linger around
        DestroyGameObject();
    }

    protected override void GatherInput()
    {
        LookAngle += Input.AnalogLook * speed;

        LookAngle.roll = 0;
        LookAngle.pitch = LookAngle.pitch.Clamp( -90, 90 );
    }

    protected override void Move()
    {
        // Rotate around the body ragdoll at a radius
        camera.WorldPosition = position + LookAngle.ToRotation() * vecRadius;
    }

    protected override void Rotate()
    {
        camera.WorldRotation = Rotation.LookAt( position - camera.WorldPosition );
    }

    private void Respawn()
    {
        Disable();

        PlayerController.Local
            .GetComponentInChildren<CharacterSpawner>( includeDisabled: true )
            .Respawn( Spawner.GetSpawnPoint() );
    }
}
