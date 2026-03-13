namespace Shooter.UI;

public class UIPopup
{
    public int Amount { get; set; }
    public float LifeTime { get; set; }
    private IPopupConsumer Parent;

    public UIPopup( IPopupConsumer parent, float lifetime )
    {
        Parent = parent;
        LifeTime = lifetime;

        DestroyAfterTime();
    }

    private async void DestroyAfterTime()
    {
        await Parent.Task.DelaySeconds( LifeTime );
        Parent.RemovePopup( this );
    }

}
