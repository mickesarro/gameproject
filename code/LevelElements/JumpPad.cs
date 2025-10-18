using Sandbox;

/// <summary>
/// Simple bounce pad that applies the specified amount of force to pad up direction
/// </summary>
public sealed class JumpPad : Component, Component.ITriggerListener
{
	[Property] private float Force { get; set; } = 100f;

	public void OnTriggerEnter( Collider other )
	{
		if ( other.IsProxy ) return;

		other.GetComponent<ICharacterBase>()
			?.ApplyForce( WorldTransform.Up * Force );
	}

}
