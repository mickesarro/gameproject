using Sandbox.Network;
using System.Threading.Tasks;

namespace Shooter.Tutorial;

/// <summary>
/// Only for tutorials to make lobby private and not pollute the server list
/// </summary>
public sealed class TutorialNetworkHelper : NetworkHelper
{
    protected override async Task OnLoad()
    {

        if ( Scene.IsEditor )
            return;

        if ( StartServer && !Networking.IsActive )
        {
            LoadingScreen.Title = "Creating Lobby";
            await Task.DelayRealtimeSeconds( 0.1f );

            var gamemode = ResourceLibrary.Get<PrefabFile>( GameMode.Current );

            var lobbyConfig = new LobbyConfig
            {
                MaxPlayers = gamemode.GetMetadata( "MaxPlayers" ).ToInt( Default: 1 ),
                Privacy = LobbyPrivacy.Private,
                Name = gamemode.GetMetadata( "Name" )
            };

            Networking.CreateLobby( lobbyConfig );
        }
    }
}
