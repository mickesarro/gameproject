using System;

namespace Shooter.NPC;

/// <summary>
/// NPC state for patrolling between pre-definend waypoints.
/// </summary>
public class PatrolState : NPCBaseState {

    private List<GameObject> waypoints;
    private int waypointCount = 0;
	private Random rand = new();

    public PatrolState(NPCController controller, StateMachine stateMachine)
        : base(controller, stateMachine) {}

    public override void OnEnter() {
        // controller.Animator.SetBool("Walk", true);
        // controller.Animator.SetBool("Run", false);
        this.waypoints = controller.Waypoints;
		waypointCount = waypoints.Count;
		controller.Agent.MoveTo( waypoints[rand.Next( 0, waypointCount )].WorldPosition );
	}

    public override void OnExit(IState nextState) {
        
    }

    public override void OnUpdate() {
        if (waypoints.Count == 0) return;

		float dist = controller.Agent.TargetPosition != null
			? ((Vector3)controller.Agent.TargetPosition - controller.Agent.AgentPosition).Length
			: -1f;

		if ( dist < controller.agentProxThreshold ) {
			controller.Agent.MoveTo( waypoints[rand.Next( 0, waypointCount )].WorldPosition );
		}
	}

}
