using Models.KindOfChats;
using OpenAI.ObjectModels.RequestModels;

namespace Service;

public interface IGptService
{
	public Task<string> AskQuestionAsync(History? history, string question);
	public List<ChatMessage> SetChatMessage(History? history, string question);
}