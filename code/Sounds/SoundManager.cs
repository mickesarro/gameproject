using System;
using Sandbox;
using Sandbox.Audio;

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
    }

    private static readonly Dictionary<SoundType, string> SoundPaths = new()
    {
        {SoundType.GunshotAR, "sounds/weapons/m4a1 shot.sound" },
        {SoundType.Explosion, "sounds/weapons/explosion_urban.sound"},
        {SoundType.Hitmarker, "sounds/weapons/hitmarker.sound"},
        {SoundType.GunshotRocket, "sounds/weapons/rocket_launcher/rocketlauncherlaunchbit.sound_c"},
        {SoundType.OutOfAmmo, "sounds/hits/nope.sound"},
    };

    /// <summary>
    /// Plays sounds globally
    /// </summary>
    public static void PlayGlobal( SoundType type, Vector3 position, float range = 2000f, float volume = 1f )
    {
		if ( type == SoundType.None ) return;

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
		if ( type == SoundType.None ) return;

        if ( !SoundPaths.TryGetValue( type, out var path ) )
        {
            Log.Warning( $"Unknown sound type {type}" );
            return;
        }

        //var cam = Game.ActiveScene?.Camera;
        //var pos = cam?.WorldPosition ?? Vector3.Zero;

        var sound = Sound.Play( path );
        sound.Distance = 0f;
        sound.Falloff = 0f;
        sound.Volume = volume;

        //sound.UI = true;

    }

}
