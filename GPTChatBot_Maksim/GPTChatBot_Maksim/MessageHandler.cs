using Assistant;
using Commands;

namespace Handlers
{
    /// <summary>
    /// Обработчик текстовых команд или текстовых сообщении бота
    /// </summary>
    public class MessageHandler(TelegramBotClient client, AI_AssistantLogic ai, CommandManager cmdManager) : BaseHandler(client)
    {
        private readonly AI_AssistantLogic ai_assistant = ai;
        private readonly CommandManager commandManager = cmdManager;

        /// <summary>
        /// Обработка текстовых команд/сообщении
        /// </summary>
        /// <param name="update">Тип обновления.</param>
        /// <returns>Ответ на текстовую команду или сообщение.</returns>
        public async Task HandleUpdate(Update update)
        {
            if (update.Message?.Text is not { } messageText) return;

            logger.LogMessage(LogLevel.Info, $"[DEBUG]: Текст сообщения - {messageText}");

            var command = commandManager.GetTextCommand(messageText);
            if (command != null)
            {
                Console.WriteLine($"[DEBUG]: Найдена команда {command.CommandName}");
                await command.ExecuteAsync(update);
            }
            else
            {
                Console.WriteLine("[DEBUG]: Обычное сообщение, передаю для AI модели на обработку...");
                var fallback = commandManager.GetDefaultCommand();
                await fallback.ExecuteFallbackAsync(update);
            }
        }
    }
}
