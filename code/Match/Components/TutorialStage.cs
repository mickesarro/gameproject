using Shooter.Sounds;

namespace Shooter;

public enum TutorialObjectiveType
{
    ReachSpeed,
    ReachTargetArea,
    ReadInstructions
    // Shoot targets, etc.
}

public struct TutorialInstruction
{
    [Property, TextArea] 
    public string Text { get; set; }

    [Property, FilePath, Description("Pick the .mp4, .webm, or image file")] 
    public string MediaPath { get; set; }
}

public sealed class TutorialStage : Component
{
    [Property, Group("Configuration")] 
    public string StageName { get; private set; } = "New Stage";

    [Property, Group("Configuration")]
    public List<TutorialInstruction> Instructions { get; set; } = new();

    [Property, Group("Level Setup")] 
    public GameObject SpawnPoint { get; private set; }

    [Property, Group("Objective")]
    public string ObjectiveText { get; private set; } = "Complete the objective";

    [Property, Group("Objective")]
    public TutorialObjectiveType ObjectiveType { get; private set; } = TutorialObjectiveType.ReachSpeed;

    [Property, Group("Objective"), ShowIf("ObjectiveType", TutorialObjectiveType.ReachTargetArea)] 
    public Collider TargetArea { get; private set; }

    [Property, Group("Objective"), ShowIf("ObjectiveType", TutorialObjectiveType.ReachSpeed)]
    public float TargetSpeed { get; private set; } = 400f;

    public int CurrentInstructionIndex { get; private set; } = 0;
    public bool IsStageActive { get; private set; } = false;
    public bool ObjectiveCompleted { get; private set; } = false;
    public bool IsFullyComplete { get; private set; } = false;

    public TutorialInstruction CurrentInstruction => Instructions.Count > 0 ? Instructions[CurrentInstructionIndex] : default;

    public void StartStage(PlayerController player)
    {
        if (SpawnPoint != null && player != null)
        {
            player.WorldPosition = SpawnPoint.WorldPosition; 

            var lookAngle = new Vector2(SpawnPoint.WorldRotation.Pitch(), SpawnPoint.WorldRotation.Yaw());
            player.LookAngle = lookAngle;
            
            player?.Velocity = Vector3.Zero;
        }

        Log.Info($"Starting Tutorial Stage: {StageName}");
        
        IsStageActive = true;
        ObjectiveCompleted = false;
        IsFullyComplete = false;
        CurrentInstructionIndex = 0;
        
        ShowCurrentInstruction();

        TargetArea?.OnObjectTriggerEnter += IsPlayerOnArea;
    }

    public void EndStage()
    {
        IsStageActive = false;
        TargetArea?.OnObjectTriggerEnter -= IsPlayerOnArea;
    }

    protected override void OnUpdate()
    {
        if (!IsStageActive || Instructions == null || Instructions.Count == 0) 
            return;

        if (Input.Pressed("back"))
        {
            NavigateInstructions(-1);
        }
        else if (Input.Pressed("use")) 
        {
            if (ObjectiveCompleted && CurrentInstructionIndex == Instructions.Count - 1)
            {
                IsFullyComplete = true;
                return;
            }

            NavigateInstructions(1);
        }
    }

    private void NavigateInstructions(int direction)
    {
        int previousIndex = CurrentInstructionIndex;
        CurrentInstructionIndex += direction;

        // If objective is not complete, max index is the 2nd to last instruction (Count - 2)
        // If objective is complete, max index is the final instruction (Count - 1)
        int maxAllowedIndex = ObjectiveCompleted ? Instructions.Count - 1 : Instructions.Count - 2;
        
        // Failsafe in case there's only 1 instruction total
        if (maxAllowedIndex < 0) maxAllowedIndex = 0;

        if (CurrentInstructionIndex < 0) 
            CurrentInstructionIndex = 0;
        if (CurrentInstructionIndex > maxAllowedIndex) 
            CurrentInstructionIndex = maxAllowedIndex;

        if (CurrentInstructionIndex != previousIndex)
        {
            ShowCurrentInstruction();
        }
    }

    private void ShowCurrentInstruction()
    {
        if (Instructions.Count == 0) return;

        var instruction = Instructions[CurrentInstructionIndex];
        
        Log.Info($"[{StageName} - Instruction {CurrentInstructionIndex + 1}/{Instructions.Count}] {instruction.Text}");
        
        if (!string.IsNullOrWhiteSpace(instruction.MediaPath))
        {
            Log.Info($"[{StageName} Media] Displaying: {instruction.MediaPath}");
        }
    }

    private bool isObjectiveMet = false;
    public bool CheckCompletion(PlayerController playerController)
    {
        if (!IsStageActive || playerController == null) return false;

        if (IsFullyComplete) return true;

        if (ObjectiveCompleted) return false;

        switch (ObjectiveType)
        {
            case TutorialObjectiveType.ReadInstructions:
                if (CurrentInstructionIndex == Instructions.Count - 2)
                {
                    isObjectiveMet = true;
                }
                break;

            case TutorialObjectiveType.ReachSpeed:
                float CurrentSpeed = playerController.Velocity.WithZ(0).Length;
                if ( CurrentSpeed > TargetSpeed )
                {
                    isObjectiveMet = true;
                }
                break;

            case TutorialObjectiveType.ReachTargetArea:
                // IsPlayerOnArea handles this condition now
                break;
        }

        if (isObjectiveMet)
        {
            Log.Info($"[{StageName}] Objective reached! Final instruction unlocked.");
            ObjectiveCompleted = true;

            SoundManager.PlayLocal(SoundManager.SoundType.Completed, 0.4f);
            
            CurrentInstructionIndex = Instructions.Count - 1;
            ShowCurrentInstruction();
        }

        return false; 
    }

    private void IsPlayerOnArea( GameObject target )
    {
        isObjectiveMet = target.Root.Tags.Has( "player" );
    }
}
