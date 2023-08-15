using Models.KindOfChats;
using OpenAI.ObjectModels.RequestModels;

namespace Service;

public interface IGptService
{
	public Task<string> AskQuestionAsync(List<History>? history, string question);
	public List<ChatMessage> SetChatMessage(List<History>? history, string question);
}