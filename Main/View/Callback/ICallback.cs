using Models;

namespace Main.Test.Callback;

public interface ICallback
{
	Task ChatCallbackHandler(ChatModelForUser model);
	Task DocCallbackHandler(ChatModelForUser model);
}