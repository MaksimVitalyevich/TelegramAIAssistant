using Assistant;

namespace Commands
{

    public interface IDefaultTextCommand
    {
        Task ExecuteFallbackAsync(Update update);
    }

    public class DefaultTextReplyCommand(TelegramBotClient client, AI_AssistantLogic assistant) : TextCommand(client), IDefaultTextCommand
    {
        private readonly AI_AssistantLogic ai_assistant = assistant;

        public override string CommandName => "default_text";

        public override Task ExecuteAsync(Update update)
        {
            return Task.CompletedTask;
        }

        public async Task ExecuteFallbackAsync(Update update)
        {
            var chatID = update.Message.Chat.Id;
            var messageText = update.Message.Text ?? string.Empty;

            var reply = await ai_assistant.GenerateResponse(messageText, chatID);
            await bot_client.SendMessage(chatID, reply);
        }
    }
}
