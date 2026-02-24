namespace Shooter.NPC;

/// <summary>
/// A state for searching the player once its lost.
/// </summary>
public class SearchState : NPCBaseState
{
    private float checkTimer = 0f;
    private readonly float checkInterval = 0.2f;

    private readonly float maxSpeed = 300.0f;
    private readonly float maxAcceleration = 200.0f;

    public float SearchTime { get; private set; }

    private NavMeshAgent agent; // Just a convenience reference

    public SearchState( NPCController controller, StateMachine stateMachine, float searchTime = 10f )
        : base( controller, stateMachine )
    {
        this.SearchTime = searchTime;
    }

    public override void OnEnter()
    {
        controller.Agent.MoveTo( controller.lastKnownPos );
        this.agent = controller.Agent;
        agent.MaxSpeed = maxSpeed;
        agent.Acceleration = maxAcceleration;

        checkTimer = 0f;

    }

    public override void OnExit( IState nextState ) {}

    public override void OnUpdate()
    {

        /*SearchTime -= Time.Delta;
        if ( SearchTime < 0 )
        {
            stateMachine.ChangeState( stateMachine.PreviousState );
        }*/

        // Limit the amount of checks
        checkTimer += Time.Delta;
        if ( checkTimer > checkInterval )
        {
            checkTimer = 0f;

            var closest = ClosestPlayerInView();
            if ( closest != null )
            {
                controller.hunted = closest;
                stateMachine.ChangeState<AttackState>();
            }
        }

        var target = agent.TargetPosition ?? controller.WorldPosition;

        // If target still is zero vector
        var dist = target != default
            ? (target - controller.Agent.AgentPosition).Length
            : -1;

        if ( !agent.IsTraversingLink && dist < controller.agentProxThreshold )
        {
            // Wander around the last known area
            // This needs a lot of improving
            //var randPos = new Vector3( 1f, 1f, 1f ) * rand.Next( 0, 10 );
            //randPos += controller.lastKnownPos;
            var randPos = Game.ActiveScene.NavMesh.GetRandomPoint( controller.lastKnownPos, 1000f );

            agent.MoveTo( randPos ?? target );
        }
    }

    private GameObject ClosestPlayerInView()
    {
        GameObject closest = null;
        float lowest = float.MaxValue;

        // Search for the closest player in the match
        foreach ( var player in HostCharacterRegistry.Current.Characters )
        {
            if ( player == controller.GameObject) continue;

            float dist = controller.WorldPosition.DistanceSquared( player.WorldPosition );

            if ( dist < lowest && PlayerInView( player ) )
            {
                closest = player;
                lowest = dist;
            }
        }

        return closest;
    }

    private bool PlayerInView( GameObject player )
    {
        // This method is more or less a duplication of the one in NPCController.
        // The situation is subject to change, but this is just to get the search working.
        // NOTE: Perhaps the one in NPCController should be centralized elsewhere as it makes no sense there.

        if ( controller.WorldPosition.DistanceSquared( player.WorldPosition ) > (controller.detectionDistance * controller.detectionDistance) )
        {
            return false;
        }

        var dirVec = (player.WorldPosition - controller.WorldPosition).Normal;
        float dot = Vector3.Dot( controller.WorldTransform.Forward, dirVec );
        if ( dot < controller.CosHalfFOV )
        {
            return false;
        }

        var hitInfo = Game.ActiveScene.Trace
            .Ray( controller.WorldPosition, dirVec )
            .Size( controller.detectionDistance )
            .UseHitboxes()
            .WithAnyTags( "shootable" )
            .Run();

        return hitInfo.Hit;
    }

}
