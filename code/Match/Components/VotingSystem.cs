using System;

namespace Shooter.Match;

public sealed class VotingSystem : SingletonBase<VotingSystem>
{
    [Sync( SyncFlags.FromHost )] public NetList<Option> Options { get; private set; } = new();
    [Sync( SyncFlags.FromHost )] private NetDictionary<Guid, int> Votes { get; set; } = new();

    [Property] private int VotingTime = 10;
    private TimeSince Elapsed;

    public Action<string> OnVotingEnded;

    [Rpc.Host]
    public void Vote( int ind )
    {
        if ( ind < 0 || ind >= Options.Count ) return;

        // Could check for same value to reduce network load
        Votes[Rpc.CallerId] = ind;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if ( !Networking.IsHost || Elapsed < VotingTime ) return;

        int winner = DetermineWinner();

        OnVotingEnded?.Invoke( Options[winner].Ident );

    }

    private int DetermineWinner()
    {
        Dictionary<int, int> counts = new();

        foreach ( var (_, vote) in Votes )
        {
            if ( !counts.TryGetValue( vote, out var _ ) )
            {
                counts.Add( vote, 0 );
            }

            counts[vote]++;
        }

        return counts.Count > 0 ? counts.MaxBy( vote => vote.Value ).Key : 0;

    }

    protected override void OnStart()
    {
        base.OnStart();

        if ( !Networking.IsHost ) return;

        BuildOptions();
        Elapsed = 0;
        
    }

    /// <summary>
    /// Represents a singular vote from a player.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="ident"></param>
    /// <param name="index"></param>
    public struct Option( string title, string ident, int index )
    {
        public string Title = title;
        public string Ident = ident;
        public int Index = index;
    }

    private async void BuildOptions()
    {
        // This needs to be re-thought
        // Probably best is to make a directory for distinct map scenes.
        // We'd add this current main scene as e.g. container.scene.
        // Loading them dynamically from the cloud is not possible as we
        // have so many custom features.

        if ( !Networking.IsHost ) return;

        Options.Add( new Option( "Rematch", "scenes/main.scene", 0 ) );

        int index = 1;
        foreach ( var map in MatchManager.Instance.MatchGameMode.AvailableMaps )
        {
            if ( !Package.TryGetCached( map.MapName, out Package package, false ) )
            {
                package = await Package.Fetch( map.MapName, false );
            }

            if ( package == null ) continue;

            Options.Add(
                new Option(
                    package.Title,
                    "scenes/main.scene", //map.MapName,
                    index++
                )
            );
        }

    }

}
