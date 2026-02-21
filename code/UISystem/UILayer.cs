
namespace Shooter.UISystem;

/// <summary>
/// Makes different UI layers/elements possible to use with UIManager.
/// </summary>
public abstract class UILayer : PanelComponent
{
    // At the moment is inherting the PanelComponent, but if for some reason
    // another is needed or preferred, create another instance or modify.

    public virtual void Show() => GameObject.Enabled = true;

    public virtual bool IsOverlay { get; set; } = false;

    /// <summary>
    /// Override this method if layer accepts data, default is empty.
    /// </summary>
    /// <param name="data"></param>
    public virtual void Show( object data ) { }

    public virtual void Hide() => GameObject.Enabled = false;
    public virtual void Hide( object data ) => GameObject.Enabled = false;

}
