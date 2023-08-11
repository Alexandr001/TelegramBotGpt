using Main.View.Callback;

namespace Main.View.Factories;

public class CallbackFactory : IFactory<string, ICallback>
{
	public IReadOnlyDictionary<string, ICallback> RouteAndObject { get; }

	public CallbackFactory()
	{
		RouteAndObject = new Dictionary<string, ICallback>() {
				[MainRouteConstants.NEW] = new CreationCallback(),
				[MainRouteConstants.DELETE] = new DeletionCallback(),
				[MainRouteConstants.NAME] = new ContinuationCallback()
		};
	}

	public ICallback? FactoryMethod(string str)
	{
		foreach ((string? key, ICallback? value) in RouteAndObject) {
			if (str == key) {
				return value;
			}
		}
		return null;
	}
}