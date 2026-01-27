using Sandbox;

namespace Shooter;

public interface ICharacterDresser
{
    public SkinnedModelRenderer BodyRenderer { get; }

    public void ApplyClothing();
}
