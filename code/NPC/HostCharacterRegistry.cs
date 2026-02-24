using Sandbox;
using System.Collections.Generic;

namespace Shooter.NPC;

/// <summary>
/// Responsible for storing character references for NPCs.
/// Only usable on host as NPCs are owned and managed by it solely.
/// </summary>
/// <param name="scene"></param>
public sealed class HostCharacterRegistry( Scene scene ) : GameObjectSystem<HostCharacterRegistry>( scene ), IMatchEvents
{
    private readonly HashSet<GameObject> characters = new();
    public HashSet<GameObject> Characters => Networking.IsHost ? characters : [];


    void IMatchEvents.BroadcastOnKill( PlayerStats killed, DamageInfo damageInfo )
    {
        if ( !Networking.IsHost ) return;

        characters.Remove( killed.GameObject );
    }

}
