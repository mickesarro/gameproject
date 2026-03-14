using System;
using System.Linq;
using Sandbox;
using Shooter.NPC;
using Shooter.Match;

namespace Shooter.Editor;

/// <summary>
/// Can be used to run scripts in the editor.
/// </summary>
public static partial class EditorScene
{
    [ConCmd("WinGame")]
    public static void WinGame()
    {
        var stats = PlayerController.Local?.CharacterStats;
        stats?.AddScore(1);

        MatchManager.Instance?.EndGame();
    }

}
