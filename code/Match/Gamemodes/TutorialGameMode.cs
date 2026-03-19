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

    public int CurrentStageIndex { get; private set; } = 0;
    public TutorialStage CurrentStage => (Stages != null && CurrentStageIndex < Stages.Count) ? Stages[CurrentStageIndex] : null;
    private bool isCurrentStageStarted = false;

    // Editor utility for showing the player input with console command "InputOverlay true/false"
    #if DEBUG
        [ConVar( Help = "Toggle the WASD recording overlay" )]
        public static bool InputOverlay { get; set; } = false;
#endif

    protected override void OnStart()
    {
        base.OnStart();
        Log.Info("Tutorial stages count: " + Stages?.Count);

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
            CurrentStage.StartStage(localPlayer);
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

        if (CurrentStageIndex >= Stages.Count)
        {
            WinCondition(null);
        }
    }

    public override void WinCondition(PlayerStats latestScoreEvent)
    {
        MatchManager.Instance?.EndGame();
        Log.Info("Tutorial Completed!");
    }

    public override void DetermineWinners()
    {
        // GameMode needs this although not needed for a this tutorial
    }
}
