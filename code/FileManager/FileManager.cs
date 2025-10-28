using Sandbox;

public static class FileManager
{
	public static void Save<T>( T data, string name ) where T : ISerializable
	{
		string path = $"{name}.json";

		if ( data.ShouldAccumulate && FileSystem.Data.FileExists( path ) )
		{
			data.Accumulate( Load<T>( name ) );
		}
		FileSystem.Data.WriteJson( path, data );
	}

	public static void Save<T>( T data ) where T : ISerializable
	{
		Save( data, data.Name );
	}

	public static T Load<T> (string name, T defaults = default) where T : ISerializable
	{
		return FileSystem.Data.ReadJson<T>( $"{name}.json", defaults );
	}

	public static void Load<T>( ref T serializable ) where T : ISerializable
	{
		serializable = FileSystem.Data.ReadJson<T>( $"{serializable.Name}.json", serializable );
	}
}
