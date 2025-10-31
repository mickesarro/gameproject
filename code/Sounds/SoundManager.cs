using System;
using Sandbox;
using Sandbox.Audio;

public static class SoundManager
{
    public enum SoundType
    {
        GunshotAR,
        GunshotRocket,
        Reload,
        Hitmarker,
        PlayerDeath,
        Explosion,
        OutOfAmmo,
    }

    private static readonly Dictionary<SoundType, string> SoundPaths = new()
    {
        {SoundType.GunshotAR, "sounds/m4a1 shot.sound" },
        {SoundType.Explosion, "sounds/explosion_urban.sound"},
        {SoundType.Hitmarker, "sounds/soundboard/hitmarker.sound_c"},
        {SoundType.GunshotRocket, "sounds/weapons/rocket_launcher/rocketlauncherlaunchbit.sound_c"},
        {SoundType.OutOfAmmo, "sounds/hits/nope.sound"},
    };

    /// <summary>
    /// Plays sounds globally
    /// </summary>

    public static void PlayGlobal( SoundType type, Vector3 position, float range = 2000f, float volume = 1f )
    {
        if ( !SoundPaths.TryGetValue( type, out var path ) )
        {
            Log.Warning( "Unknown sound type" + type );
            return;
        }

        var sound = Sound.Play( path, position );
        sound.Distance = range;
        sound.Falloff = 0.2f;
    }

    /// <summary>
    /// Plays sounds locally
    /// </summary>

    public static void PlayLocal( SoundType type, float volume = 1f )
    {
        if ( !SoundPaths.TryGetValue( type, out var path ) )
        {
            Log.Warning( $"Unknown sound type {type}" );
            return;
        }

        var cam = Game.ActiveScene?.Camera;
        var pos = cam?.WorldPosition ?? Vector3.Zero;

        var sound = Sound.Play( path, pos );
        sound.Distance = 0f;
        sound.Falloff = 0f;
        sound.Volume = volume;

        //sound.UI = true;
        //En tiiä miks vitussa tää ei toimi
        //https://sbox.game/api/Sandbox.SoundEvent
        
        //Toimii vaa jos menee Asset browser -> cloud -> hakee hitmarker.sound_c -> Is this 2D?

    }

}