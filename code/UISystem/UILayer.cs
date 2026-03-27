
namespace Shooter.UISystem;

public abstract class UILayer : PanelComponent
{
    public virtual bool IsOverlay { get; protected set; } = false;
    public virtual int HideTimeMs { get; protected set; } = 0;

    public virtual void Show() 
    {
        GameObject.Enabled = true;
        Panel?.RemoveClass("closing");
    }

    public virtual void Show( object data ) => Show();
    public virtual bool CanShow( UILayer currentLayer ) => true;

    public virtual async void Hide()
    {
        if ( HideTimeMs <= 0 )
        {
            GameObject.Enabled = false;
            Panel?.RemoveClass("closing");
            return;
        }

        Panel?.AddClass("closing");

        // Catch an arbitrary error caused by closing the Pause Menu
        try 
        {
            await Task.Delay( HideTimeMs );
        }
        catch (System.OperationCanceledException)
        {
            return;
        }
        

        // Stop execution if the component was destroyed during the delay
        if ( !IsValid ) return;

        if ( Panel?.HasClass("closing") == true )
        {
            GameObject.Enabled = false;
            Panel?.RemoveClass("closing");
        }
    }

    public virtual void Hide( object data ) => Hide();
}
