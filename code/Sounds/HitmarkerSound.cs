namespace Shooter.Sounds;

/// <summary>
/// Draws hitmarker with plain geometry using Camera HudPainter.
/// Should be added to the HUD GameObject in the UI.
/// </summary>
public sealed class HitmarkerSound : Component, IDamageEvent
{
    void IDamageEvent.OnDamage( GameObject receiver, DamageInfo damageInfo )
    {
        // NPC inflicted the damage, hitmarker should not be shown
        // Previously displayed this on the host
        if ( damageInfo.Tags.Has( "npc" ) ) return;

        if ( damageInfo.Tags.Has( "head" ) )
        {
            SoundManager.PlayLocal( SoundManager.SoundType.Headshot );
        }
        else
        {
            SoundManager.PlayLocal( SoundManager.SoundType.Hitmarker );
        }
        
    }


}
