namespace Shooter.NPC;

/// <summary>
/// NPC state responsible for simply stationary guarding.
/// </summary>
public class GuardState : NPCBaseState { // Could be renamed to IdleState
    public GuardState(NPCController controller, StateMachine stateMachine)
        : base(controller, stateMachine) {}

    public override void OnEnter() {
        // controller.Animator.SetBool("Run", false);
        // controller.Animator.SetBool("Walk", false);
    }

    public override void OnExit(IState nextState) {
        
    }

    public override void OnUpdate() {
        
    }
}
