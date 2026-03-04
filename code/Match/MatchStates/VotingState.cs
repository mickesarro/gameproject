using Sandbox;

namespace Shooter.Match;

public sealed class VotingState( MatchManager matchManager, StateMachine stateMachine )
    : MatchBaseState( matchManager, stateMachine ), Component.INetworkListener
{
    public override StateEnum StateEnum => StateEnum.Voting;

    public override void OnEnter()
    {
        if ( Networking.IsHost )
        {
            var vs = GameObject.Clone(
                "prefabs/VotingSystem.prefab",
                new CloneConfig { StartEnabled = true, Parent = matchManager.GameObject }
            );

            vs.NetworkSpawn();

            vs.GetComponent<VotingSystem>().OnVotingEnded += StartNewMatch;
        }
    }

    void Component.INetworkListener.OnBecameHost( Connection previousHost )
    {
        VotingSystem.Instance.OnVotingEnded += StartNewMatch;
    }

    public override void OnExit( IState nextState ) {}

    public override void OnUpdate() { }

    private void StartNewMatch( string ident )
    {
        if ( !Networking.IsHost ) return;

        var slo = new SceneLoadOptions();
        slo.SetScene( ident );
        Game.ChangeScene( slo );
    }

}

