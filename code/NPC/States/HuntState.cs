namespace Shooter.NPC;

/// <summary>
/// NPC state responsible for hunting the player when its in sight.
/// </summary>
public class HuntState : NPCBaseState
{

    private GameObject hunted;

    private float checkTimer = 0f;
    private readonly float checkInterval = 0.2f;

    private readonly float maxSpeed = 350.0f;
    private readonly float maxAcceleration = 300.0f;

    public HuntState( NPCController controller, StateMachine stateMachine )
        : base( controller, stateMachine ) { }

    public override void OnEnter()
    {
        hunted = controller.Hunted;
        controller.lastKnownPos = hunted.WorldPosition;

        checkTimer = 0f;

        controller.Agent.MaxSpeed = maxSpeed;
        controller.Agent.Acceleration = maxAcceleration;

    }

    public override void OnExit( IState nextState ) {}

    public override void OnUpdate()
    {
        checkTimer += Time.Delta;
        if ( checkTimer > checkInterval )
        {
            checkTimer = 0;

            if ( controller.HuntedInView() )
            {
                controller.lastKnownPos = hunted.WorldPosition;

                if ( hunted.WorldPosition.Distance( controller.WorldPosition ) < controller.agentProxThreshold )
                {
                    stateMachine.ChangeState<AttackState>();
                }
            }
            else
            {
                stateMachine.ChangeState<SearchState>();
            }
        }
    }
}
