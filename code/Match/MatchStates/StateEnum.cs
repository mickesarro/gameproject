namespace Shooter.Match;

[System.Serializable]
public enum StateEnum {
    None,
    Start,
    Match,
    End,
    Voting,
}

public static class StateEnumExtensions
{
    public static IState CreateState(
        this StateEnum state, MatchManager matchManager, StateMachine stateMachine
            ) => state switch
        {
            StateEnum.Start => new StartState( matchManager, stateMachine ),
            StateEnum.Match => new MatchState( matchManager, stateMachine ),
            StateEnum.End => new EndState( matchManager, stateMachine ),
            StateEnum.Voting => new VotingState( matchManager, stateMachine ),
            StateEnum.None => null,
        };
}

