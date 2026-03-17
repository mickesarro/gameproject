using Sandbox;
using System.Collections.Generic;

namespace Shooter;

public enum TutorialObjectiveType
{
    ReachSpeed,
    ReachTargetArea
    // Shoot targets, etc.
}

public struct TutorialInstruction
{
    [Property, TextArea] 
    public string Text { get; set; }

    [Property, Description("Path to the .mp4 or .webm file")] 
    public string MediaPath { get; set; }
}

public sealed class TutorialStage : Component
{
    [Property, Group("Configuration")] 
    public string StageName { get; set; } = "New Stage";

    [Property, Group("Configuration")]
    public List<TutorialInstruction> Instructions { get; set; } = new();

    [Property, Group("Level Setup")] 
    public GameObject SpawnPoint { get; set; }

    [Property, Group("Objective")]
    public TutorialObjectiveType ObjectiveType { get; set; } = TutorialObjectiveType.ReachSpeed;

    [Property, Group("Objective"), ShowIf("ObjectiveType", TutorialObjectiveType.ReachTargetArea)] 
    public GameObject TargetArea { get; set; }

    [Property, Group("Objective"), ShowIf("ObjectiveType", TutorialObjectiveType.ReachSpeed)]
    public float TargetSpeed { get; set; } = 400f;

    private int currentInstructionIndex = 0;
    private bool isStageActive = false;
    
    private bool objectiveCompleted = false; 
    private bool isFullyComplete = false; 

    public void StartStage(GameObject player)
    {
        if (SpawnPoint != null && player != null)
        {
            player.WorldPosition = SpawnPoint.WorldPosition;
            player.WorldRotation = SpawnPoint.WorldRotation;
            
            var controller = player.Components.Get<PlayerController>();
            if (controller != null)
            {
                controller.Velocity = Vector3.Zero;
            }
        }

        Log.Info($"Starting Tutorial Stage: {StageName}");
        
        isStageActive = true;
        objectiveCompleted = false;
        isFullyComplete = false;
        currentInstructionIndex = 0;
        
        ShowCurrentInstruction();
    }

    public void EndStage()
    {
        isStageActive = false;
    }

    protected override void OnUpdate()
    {
        if (!isStageActive || Instructions == null || Instructions.Count == 0) 
            return;

        if (Input.Pressed("back"))
        {
            NavigateInstructions(-1);
        }
        else if (Input.Pressed("use")) 
        {
            if (objectiveCompleted && currentInstructionIndex == Instructions.Count - 1)
            {
                isFullyComplete = true;
                return;
            }

            NavigateInstructions(1);
        }
    }

    private void NavigateInstructions(int direction)
    {
        int previousIndex = currentInstructionIndex;
        currentInstructionIndex += direction;

        // If objective is not complete, max index is the 2nd to last instruction (Count - 2)
        // If objective is complete, max index is the final instruction (Count - 1)
        int maxAllowedIndex = objectiveCompleted ? Instructions.Count - 1 : Instructions.Count - 2;
        
        // Failsafe in case there's only 1 instruction total
        if (maxAllowedIndex < 0) maxAllowedIndex = 0;

        if (currentInstructionIndex < 0) 
            currentInstructionIndex = 0;
        if (currentInstructionIndex > maxAllowedIndex) 
            currentInstructionIndex = maxAllowedIndex;

        if (currentInstructionIndex != previousIndex)
        {
            ShowCurrentInstruction();
        }
    }

    private void ShowCurrentInstruction()
    {
        if (Instructions.Count == 0) return;

        var instruction = Instructions[currentInstructionIndex];
        
        Log.Info($"[{StageName} - Instruction {currentInstructionIndex + 1}/{Instructions.Count}] {instruction.Text}");
        
        if (!string.IsNullOrWhiteSpace(instruction.MediaPath))
        {
            Log.Info($"[{StageName} Media] Displaying: {instruction.MediaPath}");
        }
    }

    public bool CheckCompletion(PlayerController playerController)
    {
        if (!isStageActive || playerController == null) return false;

        if (isFullyComplete) return true;

        if (objectiveCompleted) return false;

        bool isObjectiveMet = false;

        switch (ObjectiveType)
        {
            case TutorialObjectiveType.ReachSpeed:
                float currentSpeed = playerController.Velocity.WithZ(0).Length;
                if (currentSpeed >= TargetSpeed)
                {
                    isObjectiveMet = true;
                }
                break;

            case TutorialObjectiveType.ReachTargetArea:
                if (TargetArea != null)
                {
                    var boxCollider = TargetArea.Components.Get<BoxCollider>();
                    if (boxCollider != null)
                    {
                        var playerPos = playerController.GameObject.WorldPosition;
                        var localPos = TargetArea.WorldTransform.PointToLocal(playerPos);
                        var halfSize = boxCollider.Scale * 0.5f;
                        var localBounds = new BBox(boxCollider.Center - halfSize, boxCollider.Center + halfSize);

                        if (localBounds.Contains(localPos))
                        {
                            isObjectiveMet = true;
                        }
                    }
                }
                break;
        }

        if (isObjectiveMet)
        {
            Log.Info($"[{StageName}] Objective reached! Final instruction unlocked.");
            objectiveCompleted = true;
            
            currentInstructionIndex = Instructions.Count - 1;
            ShowCurrentInstruction();
        }

        return false; 
    }
}