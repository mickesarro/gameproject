using Sandbox;

namespace Shooter;

public sealed class CheckpointTrigger : Component, Component.ITriggerListener
{
    [Property, Group("References")] public ModelRenderer CylinderRenderer { get; set; }
    [Property, Group("References")] public SpriteRenderer IconRenderer { get; set; }
    [Property, Group("References")] public SpawnPoint SpawnPointNode { get; set; }

    [Property, Group("Active Checkpoint Visuals")] public Material ActiveMaterial { get; set; }
    [Property, Group("Active Checkpoint Visuals")] public Texture ActiveSprite { get; set; }
    [Property, Group("Active Checkpoint Visuals")] public Color ActiveColor { get; set; } = Color.Green;

    public bool IsActivated { get; private set; }
    
    private TutorialStage _stage;

    private bool _isCached;
    private Sprite _defaultSprite;
    private Texture _defaultTexture;
    private Color _defaultColor;

    public void Initialize(TutorialStage stage)
    {
        _stage = stage;
        CacheDefaults();
        ResetCheckpoint();
    }

    private void CacheDefaults()
    {
        if ( _isCached ) return;

        if ( IconRenderer.IsValid() )
        {
            _defaultSprite = IconRenderer.Sprite;
            _defaultTexture = IconRenderer.Texture;
            _defaultColor = IconRenderer.Color;
        }

        _isCached = true;
    }

    public void OnTriggerEnter( Collider other )
    {
        if ( !IsActivated && other.Tags.Has( "player" ) )
        {
            ActivateCheckpoint();
        }
    }

    public void OnTriggerExit( Collider other ) { }

    public void ActivateCheckpoint()
    {
        IsActivated = true;
        
        // Visuals
        if ( CylinderRenderer.IsValid() && ActiveMaterial is not null )
        {
            CylinderRenderer.MaterialOverride = ActiveMaterial;
        }

        if ( IconRenderer.IsValid() && ActiveSprite is not null )
        {
            IconRenderer.Sprite = Sprite.FromTexture( ActiveSprite );

            IconRenderer.Color = ActiveColor;
        }

        if ( _stage != null )
        {
            _stage.RegisterCheckpoint( this );
        }
    }

    public void ResetCheckpoint()
    {
        IsActivated = false;
        
        if ( CylinderRenderer.IsValid() )
        {
            CylinderRenderer.MaterialOverride = null;
        }

        if ( IconRenderer.IsValid() )
        {
            IconRenderer.Sprite = _defaultSprite;
            IconRenderer.Texture = _defaultTexture;
            IconRenderer.Color = _defaultColor;
        }
    }
}