/*
 * This file is based on "Sauce-Movement-Base" by SmileCorp,
 * licensed under the Creative Commons Attribution 4.0 International License.
 * Original available at: https://github.com/smilefordiscord/Sauce-Movement-Base.git
 *
 * Modifications include the following:
 * - Updated deprecated S&box APIs to new ones
 *
 * License: CC BY 4.0 (https://creativecommons.org/licenses/by/4.0/)
 */

using Sandbox;

public sealed class Footsteps : Component
{
	[Property] SkinnedModelRenderer Source { get; set; }

	protected override void OnEnabled()
	{
		if ( Source is null )
			return;

		Source.OnFootstepEvent += OnEvent;
	}

	protected override void OnDisabled()
	{
		if ( Source is null )
			return;

		Source.OnFootstepEvent -= OnEvent;
	}

	TimeSince timeSinceStep;

	private void OnEvent( SceneModel.FootstepEvent e )
	{
		if ( timeSinceStep < 0.2f )
			return;

		var tr = Scene.Trace
			.Ray( e.Transform.Position + Vector3.Up * 20, e.Transform.Position + Vector3.Up * -20 )
			.Run();

		if ( !tr.Hit )
			return;

		if ( tr.Surface is null )
			return;

		timeSinceStep = 0;

		var sound = e.FootId == 0 ? tr.Surface.SoundCollection.FootLeft : tr.Surface.SoundCollection.FootRight;
		if ( sound is null ) return;

		var handle = Sound.Play( sound, tr.HitPosition + tr.Normal * 5 );
		handle.Volume *= e.Volume;
	}
}
