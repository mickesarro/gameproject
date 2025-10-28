namespace NPC;

/// <summary>
/// NPC state responsible for hunting the player when its in sight.
/// </summary>
public class HuntState : NPCBaseState
{

	private GameObject hunted;

	private float checkTimer = 0f;
	private float checkInterval = 0.2f;

	public HuntState( NPCController controller, StateMachine stateMachine )
		: base( controller, stateMachine ) { }

	public override void OnEnter()
	{
		hunted = controller.Hunted;
		controller.lastKnownPos = hunted.WorldPosition;
	}

	public override void OnExit( IState nextState ) {}

	public override void OnUpdate()
	{
		checkTimer -= Time.Delta;
		if ( checkTimer < checkInterval )
		{
			checkTimer = checkInterval;

			if ( controller.HuntedInView() )
			{
				controller.lastKnownPos = hunted.WorldPosition;
			}
			else
			{
				stateMachine.ChangeState<SearchState>();
			}
		}

		if ( hunted.WorldPosition.Distance( controller.WorldPosition ) < controller.agentProxThreshold )
		{
			stateMachine.ChangeState<AttackState>();
		}
	}
}
