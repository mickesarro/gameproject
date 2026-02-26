namespace Shooter.Camera;

/// <summary>
/// Represents the death camera after dying.
/// Player can orbit the ragdoll with this component.
/// </summary>
public sealed class OrbitCamera : BoundedCamera, ICountdownable
{
    // How far to orbit from
    [Property] public float Radius { get; set; } = 50.0f;
    private Vector3 vecRadius = Vector3.Zero;

    private TimeSince TimeSince;

    // Defines how long the camera remains
    [Property] public int Lifetime { get; private set; } = 10;

    private bool countdownActive = false;
    public bool IsActive => countdownActive;
    public int GetTime() => Lifetime - TimeSince.Relative.CeilToInt();
    bool ICountdownable.Skippable => true;
    int ICountdownable.SkipTimeLeft() => MinWaitTime - TimeSince.Relative.CeilToInt();

    [Property] private int MinWaitTime = 3;

    protected override void OnEnabled()
    {
        base.OnEnabled();

        TimeSince = 0.0f;
        vecRadius = new Vector3( Radius );

        countdownActive = true;
        IMatchEvents.Post( e => e.OnCountdownStart( this ) );
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if ( TimeSince > Lifetime )
        {
            Respawn();
        }

        if ( Input.Down("Skip") && TimeSince > MinWaitTime )
        {
            Respawn();
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

    public override void Disable()
    {
        base.Disable();

        countdownActive = false;
    }

    private void Respawn()
    {
        Disable();

        PlayerController.Local
            .GetComponentInChildren<CharacterSpawner>( includeDisabled: true )
            .Respawn( Spawner.GetSpawnPoint() );
    }
}
