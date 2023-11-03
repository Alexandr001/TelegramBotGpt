using System.Reflection;

public static class TransientIoC
{
	private static readonly Dictionary<Type, Type> _transientObjects = new();

	public static void RegisterService<T1, T2>()
			where T1 : class
			where T2 : T1, new()
	{
		_transientObjects.Add(typeof(T1), typeof(T2));
	}

	public static T GetService<T>()
			where T : class
	{
		T? instance = Activator.CreateInstance(_transientObjects[typeof(T)]) as T;
		if (instance is null) {
			throw new NullReferenceException($"Failed to convert type {_transientObjects[typeof(T)]} to type {nameof(T)}");
		}
		return instance;
	}
}