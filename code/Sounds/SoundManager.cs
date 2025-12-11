using Sandbox;
using Sandbox.Audio;

namespace Shooter.Sounds;

/// <summary>
/// Centralized manager for playing audio.
/// </summary>
public static class SoundManager
{
    public enum SoundType
    {
        None,
        GunshotAR,
        GunshotRocket,
        Reload,
        Hitmarker,
        PlayerDeath,
        Explosion,
        OutOfAmmo,
        Punch,
        UIAccept,
        UIReject,
    }

    private static readonly Dictionary<SoundType, string> SoundPaths = new()
    {
        {SoundType.GunshotAR, "sounds/weapons/m4a1 shot.sound" },
        {SoundType.Explosion, "sounds/weapons/explosion_urban.sound"},
        {SoundType.Hitmarker, "sounds/weapons/hitmarker.sound"},
        {SoundType.GunshotRocket, "sounds/weapons/rocket_launch.sound"},
        {SoundType.Reload, "sounds/weapons/m4 reload sequence.sound"},
        {SoundType.OutOfAmmo, "sounds/weapons/rifle-clip-empty.sound"},
        {SoundType.Punch, "sounds/weapons/punch-miss.sound"},
        {SoundType.UIAccept, "sounds/ui/ui_accept.sound" },
        {SoundType.UIReject, "sounds/ui/ui_accept.sound" } // No UI reject sound yet
        //{SoundType.GunshotRocket, "sounds/weapons/rocket_launcher/rocketlauncherlaunchbit.sound_c"},
        //{SoundType.OutOfAmmo, "sounds/hits/nope.sound"},
    };

    /// <summary>
    /// Plays sounds globally using the predefined dictionary
    /// </summary>
    public static void PlayGlobal( SoundType type, Vector3 position, float range = 2000f, float volume = 1f )
    {
        if ( type == SoundType.None ) return;

        if ( !SoundPaths.TryGetValue( type, out var path ) )
        {
            Log.Warning( $"Unknown sound type {type}" );
            return;
        }

        var sound = Sound.Play( path, position );
        if ( sound != null )
        {
            sound.Distance = range;
            sound.Falloff = 0.2f;
        }

    }

    /// <summary>
    /// Play any sound globally that is provided as a sound event
    /// </summary>
    /// <param name="soundEvent"></param>
    /// <param name="position"></param>
    /// <param name="range"></param>
    /// <param name="volume"></param>
    public static void PlayGlobal( SoundEvent soundEvent, Vector3 position, float range = 2000f, float volume = 1f )
    {
        if ( soundEvent == null )
        {
            Log.Warning( "Provided sound event null." );
            return;
        }

        var sound = Sound.Play( soundEvent, position );
        if ( sound != null )
        {
            sound.Distance = range;
            sound.Falloff = 0.2f;
        }
    }

    /// <summary>
    /// Plays sounds locally using the predefined dictionary
    /// </summary>
    public static void PlayLocal( SoundType type, float volume = 1f )
    {
        if ( type == SoundType.None ) return;

        if ( !SoundPaths.TryGetValue( type, out var path ) )
        {
            Log.Warning( $"Unknown sound type {type}" );
            return;
        }

        //var cam = Game.ActiveScene?.Camera;
        //var pos = cam?.WorldPosition ?? Vector3.Zero;

        var sound = Sound.Play( path );
        if ( sound != null )
        {
            sound.Distance = 10f;
            sound.Falloff = 0f;
            sound.Volume = volume;
        }


        //sound.UI = true;

    }

}
