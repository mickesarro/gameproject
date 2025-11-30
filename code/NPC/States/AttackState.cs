using System;

namespace Shooter.NPC;

/// <summary>
/// NPC state responsible for simply stationary guarding.
/// </summary>
public class AttackState : NPCBaseState
{
	public AttackState( NPCController controller, StateMachine stateMachine, GameObject player )
		: base( controller, stateMachine ) {}

	public override void OnEnter()
	{
		if ( controller.gun == null )
		{
			stateMachine.ChangeState( stateMachine.PreviousState );
		}
	}

	public override void OnExit( IState nextState ) {}

	// Could be made to TimeSince
	private float checkTimer = 0f;
	private float checkInterval = 0.2f;
	private float memoryTimer = 0f;
	private float memoryDuration = 5f;
	public override void OnUpdate()
	{
		checkTimer += Time.Delta;
		memoryTimer += Time.Delta;

		if ( checkTimer > checkInterval )
		{
			checkTimer = 0f;
			
			if ( controller.HuntedInView() )
			{
				// Player seen, try to shoot it

				memoryTimer = 0f;
				controller.lastKnownPos = controller.hunted.WorldPosition;
				controller.Agent.MoveTo( controller.hunted.WorldPosition );

				if ( controller.gun.CanShoot() )
				{
					controller.gun.Attack();
				}
			}
			else if ( memoryTimer < memoryDuration )
			{
				Movement();
			}
			else // Begin the search again
			{
				stateMachine.ChangeState<SearchState>();
			}
		}
	}

	private void Movement()
	{
		if ( controller.Agent.IsTraversingLink ) return;

		// Just some random movement, the area can be adjusted
		var newPos = controller.lastKnownPos + RandomVector( higher: 200 ) * controller.WorldTransform.Forward;

		controller.Agent.MoveTo( newPos );
	}

	private static readonly Random rand = new();
	private static Vector3 RandomVector(int lower = 0, int higher = 100)
	{
		return new Vector3( 1f, 1f, 1f ) * rand.Next( lower, higher );
	}
}
