
namespace Shooter;

/// <summary>
/// Interface to make states work with the state machine.
/// </summary>
public interface IState {
    public void OnEnter();
    public void OnExit(IState nextState);
    public void OnUpdate();
}
