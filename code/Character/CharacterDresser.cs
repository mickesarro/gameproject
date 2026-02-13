using Sandbox;

namespace Shooter;

public interface ICharacterDresser
{
    public SkinnedModelRenderer BodyRenderer { get; }

    public void SaveClothing();
    
    public void ApplyClothing();

    public void ClearClothing();

    public float tint { get; }
}
