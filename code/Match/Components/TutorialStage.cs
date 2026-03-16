using Sandbox;

namespace Shooter;

public enum TutorialObjectiveType
{
    ReachSpeed,
    ReachTargetArea
    // Shoot targets, etc.
}

public sealed class TutorialStage : Component
{
    [Property, Group("Configuration")] 
    public string StageName { get; set; } = "New Stage";

    [Property, Group("Configuration")] 
    public string InstructionText { get; set; } = "Do the thing.";

    [Property, Group("Configuration")] 
    public string MediaPath { get; set; } // Video or image

    [Property, Group("Level Setup")] 
    public GameObject SpawnPoint { get; set; }

    [Property, Group("Objective")]
    public TutorialObjectiveType ObjectiveType { get; set; } = TutorialObjectiveType.ReachSpeed;

    [Property, Group("Objective"), ShowIf("ObjectiveType", TutorialObjectiveType.ReachTargetArea)] 
    public GameObject TargetArea { get; set; }

    [Property, Group("Objective"), ShowIf("ObjectiveType", TutorialObjectiveType.ReachSpeed)]
    public float TargetSpeed { get; set; } = 400f;

    public void StartStage(GameObject player)
    {
        if (SpawnPoint != null && player != null)
        {
            player.WorldPosition = SpawnPoint.WorldPosition;
            player.WorldRotation = SpawnPoint.WorldRotation;
            
            // Stop momentum after teleporting
            var controller = player.Components.Get<PlayerController>();
            if (controller != null)
            {
                controller.Velocity = Vector3.Zero;
            }
        }

        Log.Info($"Starting Tutorial Stage: {StageName}");
        // Trigger UI
    }

    public bool CheckCompletion(PlayerController playerController)
    {
        if (playerController == null) return false;

        switch (ObjectiveType)
        {
            case TutorialObjectiveType.ReachSpeed:
                float currentSpeed = playerController.Velocity.WithZ(0).Length;
                
                if (currentSpeed >= TargetSpeed)
                {
                    Log.Info($"Speed objective reached: {currentSpeed}");
                    return true;
                }
                break;

            case TutorialObjectiveType.ReachTargetArea:
                if (TargetArea == null) return false;

                var boxCollider = TargetArea.Components.Get<BoxCollider>();
                
                if (boxCollider == null) return false;
                
                var playerPos = playerController.GameObject.WorldPosition;
                var localPos = TargetArea.WorldTransform.PointToLocal(playerPos);

                // Box collider bounds
                var halfSize = boxCollider.Scale * 0.5f;
                var localBounds = new BBox(boxCollider.Center - halfSize, boxCollider.Center + halfSize);

                if (localBounds.Contains(localPos))
                {
                    Log.Info("Area objective reached!");
                    return true;
                }
            break;
        }

        return false; 
    }
}