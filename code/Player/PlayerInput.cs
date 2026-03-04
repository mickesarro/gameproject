using Sandbox;
using Shooter.UISystem;
using Shooter.UI;
using Shooter.Camera;

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

    private HUD HUD;

    protected override void OnStart()
    {
        base.OnStart();

        if ( IsProxy ) return;

        HUD = Scene.Get<HUD>();
    }

	protected override void OnUpdate()
	{
		if ( IsProxy ) return;

        if ( Input.Suppressed ) return;

		PollItemChange();
		PollUIToggle();
	}

	private void PollUIToggle()
	{
		if ( Input.Down( "OpenStats" ) )
		{
			if ( UIManager.Instance.CurrentLayer is not StatsUI )
            {
                UIManager.Instance.ToggleLayer<StatsUI>();
            }
		}
        else if ( UIManager.Instance.CurrentLayer is StatsUI )
        {
            UIManager.Instance.ToggleLayer<StatsUI>();
        }

		if ( Input.Pressed( "Menu" ) )
		{
			UIManager.Instance.ToggleLayer<PauseMenu>();
		}

#if DEBUG
        if ( !Application.IsEditor ) return;

        if ( Input.Pressed( "Suicide" ) )
        {
            GameObject.GetComponent<CharacterHealth>().TakeDamage( new DamageInfo { Damage = 10000000, Attacker = GameObject } );
        }

        if ( Input.Keyboard.Down( "PGUP" ) )
        {
            GameObject.GetComponent<CharacterHealth>().Health = 10000000;
            HUD?.Hide();

            foreach ( var item in GameObject.GetComponentsInChildren<ModelRenderer>() )
            {
                Log.Info( item.GameObject.Parent.Name );
                if ( item.GameObject.Parent.Name == "Melee" )
                {
                    item.GameObject.Enabled = false;
                }

                item.RenderType = ModelRenderer.ShadowRenderType.On;
            }

            GameObject.GetComponent<PlayerController>().Enabled = false;

            var fc = CameraType.Flying.CreateCamera();
            fc.GameObject.Enabled = true;
            this.Enabled = false;
        }
        #endif
    }

	private void PollItemChange() {
		for ( int i = 0; i < 4; i++ )
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
