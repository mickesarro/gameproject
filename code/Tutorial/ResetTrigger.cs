using Sandbox;

namespace Shooter;

public sealed class ResetTrigger : Component, Component.ITriggerListener
{
    private TutorialStage _stage;

    // Called by the TutorialStage to link this trigger to the active stage
    public void Initialize(TutorialStage stage)
    {
        _stage = stage;
    }

    public void OnTriggerEnter( Collider other )
    {
        // Reset the player if they touch the collider
        if ( other.Tags.Has( "player" ) )
        {
            _stage?.ReturnToCheckpoint();
        }
    }

    public void OnTriggerExit( Collider other ) { }
}