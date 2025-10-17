using Sandbox;

public interface ISerializable
{
	public string Name { get; }

	// Empty on purpose.
	// Used to mark types that are safe to serialize via FileManager.
}
