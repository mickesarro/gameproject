namespace Shooter;

public interface ICountdownable
{
    int GetTime();
    bool IsActive { get; }

    bool Skippable { get; }
    int SkipTimeLeft() => 0;
}
