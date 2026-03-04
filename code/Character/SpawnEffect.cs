﻿using System;
 using Sandbox.Citizen;

namespace Shooter.Helpers;

public class SpawnEffect : Component
{
    [Property] private CitizenAnimationHelper originalHelper { get; set; }
    private Shooter.PlayerController PlayerController;

    private bool shouldRender;
    private ICharacterDresser dresser;
    [Property] private SkinnedModelRenderer originalRenderer { get; set; }
    private Material translucentSkin;
    private float skinFadedinOpacity = 0f;

    [Property] private SkinnedModelRenderer tempRenderer { get; set; }
    private bool effectFadedIn = false;
    private float opacity = 0f;

    [Property] private CitizenAnimationHelper tempHelper { get; set; }
    
    [Property] private float tick { get; set; } = 0f;

protected override void OnAwake()
    {
        base.OnAwake();
        Enabled = true;
        // translucent material doesn't have shadows, so use these 
        originalRenderer.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
        // originalRenderer.Enabled = false;
        
        dresser = GetComponent<ICharacterDresser>();
        // dresser.ClearClothing();
        
        var emptyMaterial = Material.Load( "materials/empty.vmat" );
        emptyMaterial.Set( "g_flOpacityScale", 0f );
        
        translucentSkin = Material.Load( "materials/test.vmat" ).CreateCopy();
        // originalRenderer.MaterialOverride = translucentSkin;
        originalRenderer.Materials.SetOverride( 0, translucentSkin );
        originalRenderer.Materials.SetOverride( 1, emptyMaterial );
        originalRenderer.Materials.SetOverride( 2, emptyMaterial  );
        
        // originalRenderer.SetMaterialOverride( translucentSkin, "skin" );
        // originalRenderer.MaterialOverride = translucentSkin;
        
        translucentSkin.Set( "g_flOpacityScale", 0f );
        
        tempRenderer.Model = originalRenderer.Model;
        tempHelper.Target = tempRenderer;
        
        var material = Material.Load( "materials/new.vmat" );
        tempRenderer.MaterialOverride = material;
        // tempRenderer.Materials.SetOverride( 0, material );
        // tempRenderer.Materials.SetOverride( 1, emptyMaterial );
        // tempRenderer.Materials.SetOverride( 2, emptyMaterial );

        // foreach ( var VARIABLE in material.Shader.Schema.Variables )
        // {
        //     Log.Info( VARIABLE.Name );
        //     Log.Info( VARIABLE.AttributeName );
        // }
        
        tempRenderer.Enabled = true;
        tempRenderer.Attributes.Set( "Opacity", 0f );
    }

    protected override void OnStart()
    {
        base.OnStart();
        shouldRender = Network.IsProxy || GameObject.Tags.Has( "npc");
        // if ( finished )
        // {
        //     originalRenderer.RenderType = ModelRenderer.ShadowRenderType.On;
        //     originalRenderer.Materials.SetOverride( 0, null );
        //     originalRenderer.Materials.SetOverride( 1, null );
        //     originalRenderer.Materials.SetOverride( 2, null );
        //     dresser.ApplyClothing();
        //     Enabled = false;
        // }
        
        tempRenderer.RenderType =
            shouldRender ? ModelRenderer.ShadowRenderType.On : ModelRenderer.ShadowRenderType.ShadowsOnly;

        tempHelper.MoveStyle = originalHelper.MoveStyle;
        GameObject.Parent?.Components?.TryGet<Shooter.PlayerController>( out PlayerController );
    }

    
    const float tickIntensity = 1f;
    private float blurIntensity = 20f;
    
    const float BlurRate          = 0.15f;
    const float FadeInRate        = 3.0f;
    const float FadeOutRate       = 0.45f;
    const float SkinFadeRate      = 0.06f;

    // ---- Phase thresholds (derived, not magic) ----
    const float FadeInEndTick  = 100f / FadeInRate;
    const float FadeOutStart   = FadeInEndTick;
    const float FadeOutEndTick = FadeOutStart + (100f / FadeOutRate);
    
    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        // Advance global tick (NEVER reset)
        tick += tickIntensity;

        // ---- Blur ----
        blurIntensity = 20f - tick * BlurRate;
        tempRenderer.Attributes.Set( "Intensity", blurIntensity );

        // ---- Opacity ----
        if ( tick < FadeInEndTick )
        {
            // Fade in
            opacity = tick * FadeInRate;
        }
        else
        {
            effectFadedIn = true;

            // Fade out
            float fadeOutTick = tick - FadeOutStart;
            opacity = 100f - fadeOutTick * FadeOutRate;
        }
        tempRenderer.Attributes.Set( "Opacity", opacity / 100f );

        // ---- Skin fade after blur settles ----
        if ( blurIntensity <= 2f )
        {
            skinFadedinOpacity = MathF.Min(
                skinFadedinOpacity + SkinFadeRate * tickIntensity,
                1.0f
            );

            translucentSkin.Set( "g_flOpacityScale", skinFadedinOpacity );
        }

        // ---- Restore materials once skin is fully visible ----
        if ( skinFadedinOpacity >= 1.0f )
        {
            originalRenderer.Materials.SetOverride( 0, null );
            originalRenderer.Materials.SetOverride( 1, null );
            originalRenderer.Materials.SetOverride( 2, null );
            tempRenderer.Enabled = false;
            tempHelper.Enabled = false;
        }

        // ---- Cleanup ----
        if ( tick >= FadeOutEndTick )
        {
            originalRenderer.Materials.SetOverride( 0, null );
            originalRenderer.Materials.SetOverride( 1, null );
            originalRenderer.Materials.SetOverride( 2, null );
            Enabled = false;
            DestroyAll();
        }
    }


    protected override void OnUpdate()
    {
        tempRenderer.Parameters.Set( "b_jump", originalRenderer.Parameters.GetBool( "b_jump" ) );
        tempRenderer.Parameters.Set( "b_attack", originalRenderer.Parameters.GetBool( "b_attack" ) );
        tempRenderer.Parameters.Set("holdtype", originalRenderer.Parameters.GetInt( "holdtype" ) );
        if ( PlayerController != null )
        {
            tempHelper.WithLook( PlayerController.SmoothLookAngleAngles.Forward );
            tempHelper.WithVelocity( PlayerController?.Velocity ?? 0 );
            tempHelper.IsGrounded = PlayerController?.IsOnGround ?? true;
        }
        tempHelper.DuckLevel = originalHelper.DuckLevel;
        tempHelper.AimBodyWeight = originalHelper.AimBodyWeight;
        tempHelper.AimEyesWeight = originalHelper.AimEyesWeight;
        tempHelper.AimHeadWeight = originalHelper.AimHeadWeight;
        tempHelper.BodyWeight = originalHelper.BodyWeight;
        if ( blurIntensity <= 2f ) originalRenderer.RenderType = shouldRender ? ModelRenderer.ShadowRenderType.On : ModelRenderer.ShadowRenderType.ShadowsOnly;
    }
    
    void DestroyAll()
    {
        dresser.ApplyClothing();
        originalRenderer.RenderType = shouldRender ? ModelRenderer.ShadowRenderType.On : ModelRenderer.ShadowRenderType.ShadowsOnly;
        // tempRenderer.Destroy();
        // tempHelper.Destroy();
        //Destroy();
    }
}
