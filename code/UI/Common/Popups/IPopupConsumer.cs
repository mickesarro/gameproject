namespace Shooter.UI;

public interface IPopupConsumer
{
    public abstract void RemovePopup( UIPopup popup );
    public abstract TaskSource Task { get; }
}
