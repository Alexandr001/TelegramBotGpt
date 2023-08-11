namespace IoC;

public static class IoCContainer
{
	private static readonly Dictionary<Type, object> _singletonObjects = new();
	private static Dictionary<string, object> _constants = new();
	public static T GetService<T>() where T : class => 
			(T) _singletonObjects[typeof(T)];

	public static T GetConstant<T>(string name) => 
			(T) _constants[name];

	public static void RegisterConstants(Dictionary<string, object> constants) => _constants = constants;

	public static void RegisterConstant(string name, object value) => 
			_constants.Add(name, value);

	public static void RegisterService<T1, T2>()
			where T1 : class
			where T2 : T1, new()
	{
		_singletonObjects.Add(typeof(T1), new T2());
	}
	
	public static void RegisterService<T1, T2>(T2 obj)
			where T1 : class
			where T2 : T1
	{
		_singletonObjects.Add(typeof(T1), obj);
	}
	
	public static void RegisterService<T>() 
			where T : class, new()
	{
		_singletonObjects.Add(typeof(T), new T());
	}
	
	public static void RegisterService<T>(T obj) 
			where T : class
	{
		_singletonObjects.Add(typeof(T), obj);
	}
}