using Sandbox;
using Sandbox.Rendering;

public enum CrosshairType
{
	Standard, // Four lines
	Dot, // Single dot in center
	Circle, // Circle with a dot inside
	TStyle // German style three line without top one
}

// TODO: Best to implement possibly in own file so that it can be
// triggered upon hits in some way
public enum HitmarkerType { Cross }

// Could be implemented with Razor, but as per https://sbox.game/dev/doc/systems/ui/hudpainter/
// using this method is more efficient as we only draw geometry here.

/// <summary>
/// Draws crosshair with plain geometry using Camera HudPainter.
/// Should be added to the HUD GameObject in the UI.
/// </summary>
public sealed class CrosshairNew : Component, IPlayerEvent
{
	private GunData gunData;
	private PlayerInventory inventory;

	// Later add an option to settings and get this via player preferences
	[Property] private CrosshairType CrosshairType { get; set; } = CrosshairType.Standard;

	protected override void OnUpdate()
	{
		if ( inventory.CurrentWeapon == null || Scene.Camera == null )
			return;

		var hudPainter = Scene.Camera.Hud;
		var center = Screen.Size * 0.5f;

		switch ( CrosshairType )
		{
			case CrosshairType.Standard:
			case CrosshairType.TStyle:
				Lines( hudPainter, center );
				break;
			case CrosshairType.Dot:
				Dot( hudPainter, center );
				break;
			case CrosshairType.Circle:
				Circle( hudPainter, center );
				break;
		}
	}

	void IPlayerEvent.OnSpawn( GameObject player )
	{
		if ( player.IsProxy ) return;

		gunData = player.GetComponent<GunData>();
		inventory = player.GetComponent<PlayerInventory>();
	}

	private void Lines( HudPainter hudPainter, Vector2 center )
	{
		// Define later in crosshair settings, these are just placeholders until then
		float gap = 10f;
		float length = 50f;
		float width = 10f;
		Color color = Color.White;

		hudPainter.DrawLine( center + Vector2.Left * (length + gap), center + Vector2.Left * gap, width, color );
		hudPainter.DrawLine( center + Vector2.Right * (length + gap), center + Vector2.Right * gap, width, color );
		hudPainter.DrawLine( center + Vector2.Down * (length + gap), center + Vector2.Down * gap, width, color );

		if ( CrosshairType == CrosshairType.Standard )
		{
			hudPainter.DrawLine( center + Vector2.Up * (length + gap), center + Vector2.Up * gap, width, color );
		}

	}

	private void Dot( HudPainter hudPainter, Vector2 center )
	{
		// Define later in crosshair settings, these are just placeholders until then
		float width = 10f;
		Color color = Color.White;

		hudPainter.DrawCircle( center, width, color );
	}

	private void Circle( HudPainter hudPainter, Vector2 center )
	{
		// Define later in crosshair settings, these are just placeholders until then
		float width = 5f;
		float size = 50f;
		Color color = Color.White;

		Dot( hudPainter, center );

		hudPainter.DrawRect(
			new Rect( center - (size / 2), size ),
			Color.Transparent,
			new Vector4( all: float.MaxValue ),
			new Vector4(all: width), color
		);

	}

}
