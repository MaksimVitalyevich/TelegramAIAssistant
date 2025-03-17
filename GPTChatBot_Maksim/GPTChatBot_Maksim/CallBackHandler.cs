using Commands;

namespace Handlers
{
    /// <summary>
    /// Обработчик поступаемых запросов на нажатую кнопку
    /// </summary>
    public class CallBackHandler(TelegramBotClient client, CommandManager cmdManager) : BaseHandler(client)
    {
        private readonly CommandManager commandManager = cmdManager;

        /// <summary>
        /// Обработка запросов на нажатые кнопки
        /// </summary>
        /// <param name="callback">Запрос.</param>
        /// <returns>Ответ на запрос.</returns>
        public async Task HandleCallback(CallbackQuery callback)
        {
            if (callback == null) return;

            logger.LogMessage(LogLevel.Info, $"[DEBUG]: Нажата кнопка: {callback.Data}");

            await bot_client.AnswerCallbackQuery(callback.Id);

            var command = commandManager.GetCallbackCommand(callback.Data);

            if (command != null)
            {
                Console.WriteLine($"[DEBUG]: Найдена команда - {command.CommandName}");
                await command.ExecuteAsync(callback);
            }
            else
            {
                var fallback = commandManager.GetDefaultCallbackCommand();
                await fallback.ExecuteFallbackAsync(callback);
            }
        }
    }
}
