using System;
using Sandbox.Rendering;

namespace Shooter.UI;

public enum HitmarkerType {
	Regular,
	Headshot,
	Kill
}

/// <summary>
/// Draws hitmarker with plain geometry using Camera HudPainter.
/// Should be added to the HUD GameObject in the UI.
/// </summary>
public sealed class Hitmarker : Component, IDamageEvent
{
	void IDamageEvent.OnDamage( GameObject receiver, DamageInfo damageInfo )
	{
        // NPC inflicted the damage, hitmarker should not be shown
        // Previously displayed this on the host
        if ( damageInfo.Tags.Has( "npc" ) ) return;
		
		if ( Scene.Camera == null )
			return;

		var hudPainter = Scene.Camera.Hud;

		var center = Screen.Size / 2f;

		var type = HitmarkerType.Regular;

		var health = receiver.GetComponent<CharacterHealth>();
		// This works because we calculate this before inflicting damage.
		// Using e.g. character healths IsAlive would be more idiomatic,
		// but it would be unreliable as we broadcast the damage taking methods to owner.
		if ( health != null && health.Health - damageInfo.Damage <= 0 )
		{
			type = HitmarkerType.Kill;
		}
		else if ( damageInfo.Tags.Has( "head" ) ) // No hitboxes for head specifically yet
		{
			//type = HitmarkerType.Headshot;
		}

		DrawMarker( hudPainter, center, type );
	}

	/// <summary>
	/// Draws an angled cross hitmarker.
	/// </summary>
	/// <param name="hudPainter"></param>
	/// <param name="center"></param>
	/// <param name="type"></param>
	private void DrawMarker( HudPainter hudPainter, Vector2 center, HitmarkerType type )
	{
		// Define perhaps later elsewhere, these are just placeholders until then.
		float gap = 10f;
		float length = 50f;
		float width = 5f;
		float baseAngle = 45f;

		Color color = type == HitmarkerType.Regular
			? Color.White.WithAlpha(0.8f) // Not full white
			: Color.Red;

		if ( type == HitmarkerType.Kill )
			width *= 1.2f;

		for ( int i = 0; i < 4; ++i )
		{
            var angle = (baseAngle + i * 90f) * MathF.PI / 180f;

			var dir = new Vector2( MathF.Cos( angle ), MathF.Sin( angle ) );

			hudPainter.DrawLine(
				center + dir * (length + gap),
				center + dir * gap,
				width,
				color
			);

		}
	}

}
