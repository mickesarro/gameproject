using Sandbox;
using Sandbox.Rendering;

namespace Shooter.UI;

public sealed class DmgFlash : Component, IPlayerEvent
{
    [Property] public Color FlashColor { get; set; } = Color.Red;
    [Property] public float FadeSpeed { get; set; } = 4.0f;

    private float currentOpacity = 0f;
    private CharacterHealth playerHealth;

    void IPlayerEvent.OnSpawn( GameObject player )
    {
        if ( player.IsProxy ) return;

        playerHealth = player.GetComponent<CharacterHealth>();
        
        if ( playerHealth != null )
        {
           
            playerHealth.OnDamage += HandleDamage;
        }
    }

    private void HandleDamage( DamageInfo info )
    {
        currentOpacity = 0.6f;
    }

    protected override void OnUpdate()
    {
        if ( currentOpacity <= 0 || Scene.Camera == null ) return;

        var hud = Scene.Camera.Hud;
        var screenRect = new Rect( 0, 0, Screen.Size.x, Screen.Size.y );
        

        hud.DrawRect( 
            screenRect, 
            Color.Transparent, 
            0,
            new Vector4( 15 * currentOpacity ), 
            FlashColor.WithAlpha( currentOpacity ) 
        );

        hud.DrawRect( 
            screenRect, 
            Color.Transparent, 
            0, 
            new Vector4( 80 * currentOpacity ), 
            FlashColor.WithAlpha( currentOpacity * 0.4f ) 
        );

        hud.DrawRect( 
            screenRect, 
            Color.Transparent, 
            0,
            new Vector4( 180 * currentOpacity ), 
            FlashColor.WithAlpha( currentOpacity * 0.15f ) 
      );

        currentOpacity = currentOpacity.LerpTo( 0f, Time.Delta * FadeSpeed );
    }

    protected override void OnDestroy()
    {
        if ( playerHealth != null )
        {
            playerHealth.OnDamage -= HandleDamage;
        }
    }
}