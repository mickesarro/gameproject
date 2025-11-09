using Sandbox;
using Shooter.UISystem;
using Shooter.UI;

namespace Shooter;

/// <summary>
/// A centralized component for gathering player input.
/// </summary>
public sealed class PlayerInput : Component
{
	// !!TODO: Merge either this class to main branches InputManager
	// or that to this one.

	// We could either define actions here to subscribe,
	// or maybe preferably add more events to the player event interface.

	// Some things such as player movement or mouse input should be
	// handled where they are needed. But for event based input like
	// jumping, crouching and especially toggling UI elements,
	// a centralized input handler can be a cleaner alternative.

	protected override void OnUpdate()
	{
		if ( IsProxy ) return;

		PollItemChange();
		PollUIToggle();
	}

	private void PollUIToggle()
	{
		if ( Input.Pressed( "OpenInventory" ) )
		{
			UIManager.Instance.ToggleLayer<InventoryUI>();
		}

		if ( Input.Pressed( "Menu" ) )
		{
			UIManager.Instance.ToggleLayer<PauseMenu>();
		}
	}

	private void PollItemChange() {
		for ( int i = 0; i < 3; i++ )
		{
			if ( Input.Pressed($"Slot{i + 1}" ) )
			{
				IPlayerEvent.PostToGameObject( GameObject, e => e.OnSwitchItem( (InventorySlot)i ) );
				return;
			}
		}

		if (Input.Pressed( "SlotNext" ))
		{
			IPlayerEvent.PostToGameObject( GameObject, e => e.OnSwitchItem( InventorySlot.Next ) );
		}
		else if ( Input.Pressed( "SlotPrev" ) )
		{
			IPlayerEvent.PostToGameObject( GameObject, e => e.OnSwitchItem( InventorySlot.Previous ) );
		}
	}

}
