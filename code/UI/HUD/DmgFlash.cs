namespace Shooter.UI;

public sealed class DmgFlash : Component, IPlayerEvent
{
    [Property] private Color FlashColor { get; set; } = Color.Red;
    [Property] private float FadeSpeed { get; set; } = 4.0f;
    [Property] private float LowHealthThreshold { get; set; } = 20.0f;
    [Property] private float LowHealthOpacity { get; set; } = 0.4f;

    private float currentOpacity = 0f;
    private CharacterHealth playerHealth;

    void IPlayerEvent.OnSpawn( GameObject player )
    {
        if ( player.IsProxy ) return;

        playerHealth = player.GetComponent<CharacterHealth>();
        
        playerHealth?.OnDamage += HandleDamage;
    }

    private void HandleDamage( DamageInfo _ )
    {
        currentOpacity = 0.6f;
    }

    private static readonly float epsilon = 0.01f;
    protected override void OnUpdate()
    {
        if ( currentOpacity < epsilon || Scene.Camera == null ) return;
        
        float targetMinOpacity = (playerHealth != null && playerHealth.Health <= LowHealthThreshold)
            ? LowHealthOpacity
            : 0f;

        currentOpacity = currentOpacity.LerpTo( targetMinOpacity, Time.Delta * FadeSpeed );

        if ( currentOpacity < epsilon ) return;
        
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
        playerHealth?.OnDamage -= HandleDamage;
    }
}
