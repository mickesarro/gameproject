using Sandbox;
using Shooter.Camera;
using System.Collections.Generic;

namespace Shooter;

public sealed class TutorialGameMode : GameMode
{
    [Property] public override string ModeName { get; } = "Tutorial";
    [Property] public override int ScoreLimit { get; } = 0;
    [Property] public override int MaxPlayers { get; } = 1;
    [Property] public override int MinPlayers { get; } = 1;
    [Property] public override bool PopulateWithNPCs { get; set; } = false;

    [Property] private int startCountdown { get; set; } = 0;
    public override int StartCountdown => startCountdown;

    [Property] private CameraType CameraType { get; set; } = CameraType.Orbit;
    public override CameraType Camera => CameraType;

    [Property] public List<TutorialStage> Stages { get; set; } = new();

    public static int StartingStageIndex {get; set; } = 0;
    public int CurrentStageIndex { get; private set; } = 0;
    public TutorialStage CurrentStage => (Stages != null && CurrentStageIndex < Stages.Count) ? Stages[CurrentStageIndex] : null;
    private bool isCurrentStageStarted = false;

    #if DEBUG
        // Editor utility for showing the player input with console command "InputOverlay true/false"
        [ConVar( Help = "Toggle the WASD recording overlay" )]
        public static bool InputOverlay { get; set; } = false;

        [ConCmd("tutorial_set_complete", Help = "Force the tutorial completion state (true/false)")]
        public static void SetTutorialCompleteCommand(bool isComplete)
        {
            if (SettingsManager.Instance != null)
            {
                SettingsManager.Instance.SetTutorialComplete(isComplete);
                Log.Info($"[Debug] Tutorial completion state forced to: {isComplete}");
            }
            else
            {
                Log.Warning("SettingsManager is not initialized yet.");
            }
        }
#endif

    protected override void OnStart()
    {
        base.OnStart();
        Log.Info("Tutorial stages count: " + Stages?.Count);

        CurrentStageIndex = StartingStageIndex;

        // Reset to start for the future
        StartingStageIndex = 0;

        // Force speed display for tutorial (you can still turn it off)
        SettingsManager.Instance?.SetSpeedDisplay(SpeedDisplayPosition.UnderCrosshair);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (Stages == null || Stages.Count == 0 || CurrentStageIndex >= Stages.Count) return;

        var CurrentStage = Stages[CurrentStageIndex];

        if (CurrentStage == null) return;

        var localPlayer = PlayerController.Local;
        if (localPlayer == null || localPlayer.GameObject == null) return;

        if (!isCurrentStageStarted)
        {
            CurrentStage?.StartStage(localPlayer);
            isCurrentStageStarted = true;
        }

        if (CurrentStage.CheckCompletion(localPlayer))
        {
            AdvanceStage();
        }
    }

    private void AdvanceStage()
    {
        // Disable old stage after completion
        if (CurrentStageIndex < Stages.Count)
        {
            var oldStage = Stages[CurrentStageIndex];
            oldStage.EndStage();
        }

        CurrentStageIndex++;
        isCurrentStageStarted = false;
        Log.Info("Advanced to next stage");

        // Changed this to stages.count - 1 since the last stage tells the player they completed
        if (CurrentStageIndex == (Stages.Count - 1))
        {
            WinCondition(null);
        }
    }

    public void GoToStage(int stageIndex)
    {
        if (Stages == null || stageIndex < 0 || stageIndex >= Stages.Count) 
        {
            Log.Warning($"Attempted to go to invalid tutorial stage: {stageIndex}");
            return;
        }

        // End the current stage if one is active
        if (isCurrentStageStarted && CurrentStageIndex < Stages.Count)
        {
            var oldStage = Stages[CurrentStageIndex];
            oldStage?.EndStage();
        }

        // Set the new stage index and reset the started flag
        CurrentStageIndex = stageIndex;
        isCurrentStageStarted = false; // This forces StartStage() to run on the next OnUpdate loop
        
        Log.Info($"Fast-traveled to tutorial stage: {stageIndex}");
    }

    public override void WinCondition(PlayerStats latestScoreEvent)
    {
        Log.Info("Tutorial Completed!");
        
        if (SettingsManager.Instance != null)
        {
            SettingsManager.Instance.SetTutorialComplete(true);
        }
    }

    public override void DetermineWinners()
    {
        // GameMode needs this although not needed for a this tutorial
    }
}
