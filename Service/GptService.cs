using IoC;
using Models;
using Models.KindOfChats;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;

namespace Service;

public class GptService : IGptService
{
	private readonly string _gptSecretKey;

	public GptService()
	{
		_gptSecretKey = IoCContainer.GetConstant<string>(Constants.GPT_KEY);
	}
	public async Task<string> AskQuestionAsync(List<History>? history, string question)
	{
		OpenAIService openAiService = new(new OpenAiOptions() {
				ApiKey = _gptSecretKey
		});
		ChatCompletionCreateResponse completionResult = 
				await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest {
						Messages = SetChatMessage(history, question),
						Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo
				});
		if (completionResult.Successful) {
			return completionResult.Choices.First().Message.Content;
		}
		if (completionResult.Error == null) {
			throw new Exception("Нейросетка в ошибке прислала NULL!");
		}
		return completionResult.Error.Message ?? "Упс, ошибочка...";
	}

	public List<ChatMessage> SetChatMessage(List<History>? history, string question)
	{
		List<ChatMessage> list = new();
		foreach (History t in history) {
			if (t is not null) {
				list.Add(ChatMessage.FromUser(t.UserMessage));
				list.Add(ChatMessage.FromAssistant(t.BotMessage));
			}
			list.Add(ChatMessage.FromUser(question));
			return list;
		}
		return list;
	}
}