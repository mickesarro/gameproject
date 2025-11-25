using Sandbox;

namespace Shooter;

/// <summary>
/// Basic screenshake
/// </summary>
public class ScreenShake : Component
{
    /// <summary>
    /// How long the effect lasts?
    /// </summary>
    [Property] public float Duration { get; set; } = 0f;

    /// <summary>
    /// How much will the screen shake?
    /// </summary>
    [Property] public float Magnitude { get; set; } = 0f;

    /// <summary>
    /// How much rotation will be added?
    /// </summary>
    [Property] public Rotation Rotation { get; set; } = Rotation.Identity;

    /// <summary>
    /// How much time much pass in between?
    /// </summary>
    [Property] public float Delay { get; set; } = 0f;

    public TimeSince TimeSince { get; set; } = 0; 

}
