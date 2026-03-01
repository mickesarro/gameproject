namespace Shooter.Camera;

/// <summary>
/// Represents a flying camera for e.g. spectator or admin.
/// Currently only for admin spectating.
/// </summary>
public sealed class FlyingCamera : BoundedCamera
{
    [Property] private readonly float Speed = 500f;

    protected override void OnEnabled()
    {
        base.OnEnabled();

        speed = Speed;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if ( Input.Down( "Duck" ) )
        {
            speed = Speed / 2;
        }
        else if ( Input.Down( "Slow" ) )
        {
            speed = Speed * 2;
        } else
        {
            speed = Speed;
        }

        if ( Input.Down( "Skip" ) )
        {
            Respawn();
        }
        else if ( Input.Down( "TargetLock" ) )
        {
            var ray = camera.ScreenPixelToRay( camera.ScreenRect.Center );
            var trace = Scene.Trace.Ray( ray, 10000f ).UseHitboxes().WithAllTags( "shootable" ).Run();

            FollowObject = trace.Hit ? trace.GameObject : null;
        }
        else if ( Input.Down( "TargetRelease" ) )
        {
            FollowObject = null;
        }
    }

    protected override void OnDisabled()
    {
        base.OnDisabled();

        // This is so it doesn't linger around
        DestroyGameObject();
    }

    protected override void GatherInput()
    {

        LookAngle += Input.AnalogLook;
        LookAngle = LookAngle.WithPitch( LookAngle.pitch.Clamp( -90, 90 ) );

        Direction = Input.AnalogMove;

        if ( Input.Keyboard.Down( "q" ) )
        {
            Direction.z -= speed * Time.Delta * 0.022f;
        }
        if ( Input.Keyboard.Down( "e" ) )
        {
            Direction.z += speed * Time.Delta * 0.022f;
        }

    }

    protected override void Move()
    {
        camera.WorldPosition += Direction * camera.WorldRotation * speed * Time.Delta;
    }

    protected override void Rotate()
    {
        if ( FollowObject == null )
        {
            base.Rotate();
        } else
        {
            position = FollowObject.WorldPosition;
            camera.WorldRotation = Rotation.LookAt( position - camera.WorldPosition );
        }
    }

    private void Respawn()
    {
        Disable();

        #if DEBUG
        PlayerController.Local.Enabled = true;
        return;
        #else
        PlayerController.Local
            .GetComponentInChildren<CharacterSpawner>( includeDisabled: true )
            .Respawn( Spawner.GetSpawnPoint() );
        #endif
    }
}
