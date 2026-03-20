using Sandbox;

namespace Shooter;

public sealed class CheckpointTrigger : Component
{
    private TutorialStage _stage;
    private SpawnPoint _spawnPoint;
    private Collider _collider;

    public void Initialize(TutorialStage stage, SpawnPoint spawnPoint)
    {
        _stage = stage;
        _spawnPoint = spawnPoint;
        
        _collider = Components.Get<Collider>();
        if (_collider != null)
        {
            _collider.OnObjectTriggerEnter += OnTriggerEnter; 
        }
    }

    private void OnTriggerEnter(GameObject other)
    {
        if (other.Root.Tags.Has("player"))
        {
            _stage.RegisterCheckpoint(_spawnPoint);
        }
    }

    protected override void OnDestroy()
    {
        if (_collider != null)
        {
            _collider.OnObjectTriggerEnter -= OnTriggerEnter;
        }
    }
}