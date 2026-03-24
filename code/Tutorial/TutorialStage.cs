using Shooter.Sounds;
using System;
using System.Collections.Generic;

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
    private PlayerController _activePlayer;

    // Instructions
    [Property, Group("Configuration")] public string StageName { get; private set; } = "New Stage";
    [Property, Group("Configuration")] public List<TutorialInstruction> Instructions { get; set; } = new();

    // Spawns & Checkpoints
    [Property, Group("Level Setup")] public SpawnPoint SpawnPoint { get; private set; }
    [Property, Group("Level Setup")] public List<CheckpointTrigger> Checkpoints { get; set; } = new();
    [Property, Group("Level Setup")] public GameObject ResetTrigger { get; set; }
    
    public List<CheckpointTrigger> ActiveCheckpoints { get; private set; } = new();

    // Objective variables
    [Property, Group("Objective")] public string ObjectiveText { get; private set; } = "Complete the objective";
    [Property, Group("Objective")] public TutorialObjectiveType ObjectiveType { get; private set; } = TutorialObjectiveType.ReachSpeed;
    [Property, Group("Objective"), ShowIf("ObjectiveType", TutorialObjectiveType.ReachTargetArea)] public Collider TargetArea { get; private set; }
    [Property, Group("Objective"), ShowIf("ObjectiveType", TutorialObjectiveType.ReachSpeed)] public float TargetSpeed { get; private set; }

    public int CurrentInstructionIndex { get; private set; } = 0;
    public bool IsStageActive { get; private set; } = false;
    public bool ObjectiveCompleted { get; private set; } = false;
    public bool IsFullyComplete { get; private set; } = false;

    public TutorialInstruction CurrentInstruction => Instructions.Count > 0 ? Instructions[CurrentInstructionIndex] : default;

    public void SpawnPlayer(PlayerController player, SpawnPoint position)
    {
        if (position == null || player == null) return;

        player.WorldPosition = position.Position; 
        player.LookAngle = new Vector2(position.WorldRotation.Pitch(), position.WorldRotation.Yaw());
        player.Velocity = Vector3.Zero;
    }

    public void ReturnToCheckpoint()
    {
        if (_activePlayer == null) return;

        // Grab the SpawnPointNode from the last active CheckpointTrigger
        SpawnPoint target = ActiveCheckpoints.Count > 0 ? ActiveCheckpoints[^1].SpawnPointNode : SpawnPoint;
        SpawnPlayer(_activePlayer, target);
        SoundManager.PlayLocal(SoundManager.SoundType.Morph, 0.5f);
        Log.Info($"[{StageName}] Player returned to checkpoint.");
    }

    public void StartStage(PlayerController player)
    {
        _activePlayer = player;

        EnableLevelObjects( true );
        
        IsStageActive = true;
        ObjectiveCompleted = false;
        IsFullyComplete = false;
        CurrentInstructionIndex = 0;
        
        ActiveCheckpoints.Clear(); 

        Log.Info($"Starting Tutorial Stage: {StageName}");
        SpawnPlayer(player, SpawnPoint);
        ShowCurrentInstruction();

        TargetArea?.OnObjectTriggerEnter += IsPlayerOnArea;

        // Initialize and visually reset all Checkpoint triggers
        foreach (var checkpoint in Checkpoints)
        {
            if (checkpoint == null) continue;
            checkpoint.Initialize(this); 
        }

        // Initialize Reset Triggers
        if ( ResetTrigger.IsValid() )
        {
            ResetTrigger.Enabled = true;
            
            var resetTriggers = ResetTrigger.Components.GetAll<ResetTrigger>( FindMode.EverythingInSelfAndDescendants );
            foreach (var resetTrigger in resetTriggers)
            {
                resetTrigger.Initialize(this);
            }
        }
    }

    public void EndStage()
    {
        IsStageActive = false;
        
        TargetArea?.OnObjectTriggerEnter -= IsPlayerOnArea;
        
        ActiveCheckpoints.Clear(); 

        if ( ResetTrigger.IsValid() )
        {
            ResetTrigger.Enabled = false;
        }

        EnableLevelObjects( false );

        var inventory = _activePlayer?.Components.Get<PlayerInventory>();
        inventory.Clear();
    }

    private void EnableLevelObjects( bool isEnabled )
    {
        foreach ( var child in GameObject.Children )
        {
            child.Enabled = isEnabled;
        }
    }

    public void RegisterCheckpoint(CheckpointTrigger checkpoint)
    {
        if (!IsStageActive) return;

        if (!ActiveCheckpoints.Contains(checkpoint))
        {
            ActiveCheckpoints.Add(checkpoint);
            Log.Info($"[{StageName}] Checkpoint Saved: {checkpoint.GameObject.Name}");
            SoundManager.PlayLocal(SoundManager.SoundType.Checkpoint, 0.5f);
        }
    }

    protected override void OnUpdate()
    {
        if (!IsStageActive || Instructions == null || Instructions.Count == 0) return;

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
        else if (Input.Pressed("reload"))
        {
            ReturnToCheckpoint();
        }
    }

    private void NavigateInstructions(int direction)
    {
        int previousIndex = CurrentInstructionIndex;
        CurrentInstructionIndex += direction;

        int maxAllowedIndex = ObjectiveCompleted ? Instructions.Count - 1 : Instructions.Count - 2;
        if (maxAllowedIndex < 0) maxAllowedIndex = 0;

        CurrentInstructionIndex = Math.Clamp(CurrentInstructionIndex, 0, maxAllowedIndex);

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
                isObjectiveMet = (CurrentInstructionIndex == Instructions.Count - 2);
                break;

            case TutorialObjectiveType.ReachSpeed:
                float currentSpeed = playerController.Velocity.WithZ(0).Length;
                isObjectiveMet = (currentSpeed > TargetSpeed);
                break;
                
            case TutorialObjectiveType.ReachTargetArea:
                // IsPlayerOnArea handles this condition now
                break;
        }

        if (isObjectiveMet)
        {
            MarkObjectiveComplete();
        }

        return false; 
    }

    private void MarkObjectiveComplete()
    {
        if (ObjectiveCompleted) return;

        Log.Info($"[{StageName}] Objective reached! Final instruction unlocked.");
        ObjectiveCompleted = true;

        SoundManager.PlayLocal(SoundManager.SoundType.Completed, 0.4f);
        
        CurrentInstructionIndex = Instructions.Count - 1;
        ShowCurrentInstruction();
    }

    private void IsPlayerOnArea(GameObject target)
    {
        if (!ObjectiveCompleted && target.Root.Tags.Has("player"))
        {
            MarkObjectiveComplete();
        }
    }
}