namespace Shooter.NPC;

/// <summary>
/// Abstract class for all NPC states to inherit from.
/// Implements the IState interface.
/// </summary>
public abstract class NPCBaseState : IState {
    protected NPCController controller;
    protected StateMachine stateMachine;

    public NPCBaseState(NPCController controller, StateMachine stateMachine) {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }

    public abstract void OnEnter();
    public abstract void OnExit(IState nextState);
    public abstract void OnUpdate();
}
