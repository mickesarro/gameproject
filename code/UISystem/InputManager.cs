using Sandbox;
using UISystem;

public sealed class InputManager : Component
{
    protected override void OnUpdate()
    {
        if ( Input.Pressed( "OpenInventory" ) )
        {
            UIManager.Instance.ToggleLayer<Inventory>();
        }

        if ( Input.Pressed( "Menu" ) )
        {
            UIManager.Instance.ToggleLayer<PauseMenu>();
        }
    }
}
