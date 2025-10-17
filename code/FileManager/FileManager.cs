using Sandbox;

public static class FileManager
{
	public static void Save<T>( T data, string name ) where T : ISerializable
	{
		FileSystem.Data.WriteJson( $"{name}.json", data );
	}

	public static void Save<T>( T data ) where T : ISerializable
	{
		Save( data, data.Name );
	}

	public static T Load<T> (string name) where T : ISerializable
	{
		return FileSystem.Data.ReadJson<T>( $"{name}.json" );
	}
}
