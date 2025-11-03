using Sandbox;

namespace Shooter;

/// <summary>
/// Used to mark types that are safe to serialize via FileManager.
/// </summary>
public interface ISerializable
{
	string Name { get; }

	/// <summary>
	/// Defines whether the serializable should accumulate the already saved data.
	/// </summary>
	bool ShouldAccumulate { get; }

	/// <summary>
	/// Called before saving. Implementations should merge or accumulate
	/// existing saved data into the current instance.
	/// </summary>
	/// <param name="data"></param>
	void Accumulate( ISerializable data ) { }

}
