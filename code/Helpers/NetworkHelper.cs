using Sandbox.Network;
using System.Threading.Tasks;

namespace Shooter;

/// <summary>
/// Creates a networked game lobby and assigns player prefabs to connected clients.
/// </summary>
[Title( "Custom Network Helper" )]
[Category( "Networking" )]
[Icon( "electrical_services" )]
public class NetworkHelper : Component, Component.INetworkListener
{
    /// <summary>
    /// Create a server (if we're not joining one)
    /// </summary>
    [Property] public bool StartServer { get; set; } = true;

    /// <summary>
    /// The prefab to spawn for the player to control.
    /// </summary>
    [Property] public GameObject PlayerPrefab { get; set; }

    /// <summary>
    /// A list of points to choose from randomly to spawn the player in. If not set, we'll spawn at the
    /// location of the NetworkHelper object.
    /// </summary>
    [Property] public List<SpawnPoint> SpawnPoints { get; set; }

    protected override async Task OnLoad()
    {
        if ( Scene.IsEditor )
            return;
        
        if ( StartServer && !Networking.IsActive )
        {
            LoadingScreen.Title = "Creating Lobby";
            await Task.DelayRealtimeSeconds( 0.1f );

            // Need to clone this here because MatchManager is not yet initialised
            // var gamemode = GameObject.Clone( GameMode.Current ).GetComponent<GameMode>( includeDisabled: true );

            var gamemode = ResourceLibrary.Get<PrefabFile>( GameMode.Current );

            var lobbyConfig = new LobbyConfig
            {
                MaxPlayers = gamemode.GetMetadata( "MaxPlayers" ).ToInt( Default: 1 ),
                Privacy = !Scene.IsEditor ? LobbyPrivacy.Public : LobbyPrivacy.Private,
                Name = gamemode.GetMetadata( "Name" )
            };

            Networking.CreateLobby( lobbyConfig );
        }
    }

    /// <summary>
    /// A client is fully connected to the server. This is called on the host.
    /// </summary>
    public void OnActive( Connection channel )
    {
        Log.Info( $"Player '{channel.DisplayName}' has joined the game" );
        if ( !PlayerPrefab.IsValid() )
        {
            Log.Error( "[Custom network helper] Player prefab invalid!" );
            return;
        }

        if ( SpawnPoints.Count == 0 )
        {
            SpawnPoints.Add( new SpawnPoint { WorldTransform = WorldTransform} );
        }

        Spawner.SpawnCharacter( PlayerPrefab, connection: channel, name: $"Player - {channel.DisplayName}" );

    }

    public Transform FindSpawnLocation() 
        => Spawner.GetSpawnPoint( checkForOthers: true ).WorldTransform;
}
