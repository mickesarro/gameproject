namespace Shooter;

public sealed class DestroyTimer : Component
{
    private float delay = 60.0f;
    [Property] public float Delay {
        get { return delay; }
        set
        {
            delay = value >= 0 ? value : 0;
        }
    }

    protected override void OnEnabled()
    {
        Invoke( delay, GameObject.Destroy );
    }
	
}
