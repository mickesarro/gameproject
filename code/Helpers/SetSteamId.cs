using System.Reflection.Metadata;
using Sandbox.Utility;

namespace Sandbox.Helpers;
/// <summary>
/// Set the current player's Steam Id as a tag.
/// </summary>
public sealed class SetSteamId : Component
{
	protected override void OnAwake()
	{
		Tags.Add( Steam.SteamId.ToString() );
		Log.Info("Added SteamId to: " + this.GameObject.ToString());
	}
}
