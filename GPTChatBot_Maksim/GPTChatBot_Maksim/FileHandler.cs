using Assistant;

namespace Handlers
{
    /// <summary>
    /// Обработчик файлов бота
    /// </summary>
    public class FileHandler(TelegramBotClient client, AI_AssistantLogic ai) : BaseHandler(client)
    {
        private readonly AI_AssistantLogic ai_assistant = ai;

        /// <summary>
        /// Обработка файлов. Поддерживает только текстовый формат
        /// </summary>
        /// <param name="update">Тип обновления.</param>
        /// <returns>Проанализированный файл с ответом на него.</returns>
        public async Task HandleFile(Update update)
        {
            if (update.Message?.Document == null) return;

            var fileID = update.Message.Document.FileId;
            var file = await bot_client.GetFile(fileID);
            var filePath = file.FilePath;

            Console.WriteLine($"[DEBUG]: Файл загружен: {filePath}");
            await bot_client.SendMessage(update.Message.Chat.Id, "Получил файл, провожу анализ...");

            await ai_assistant.ProcessTextFileAsync(update.Message, bot_client);
        }
    }
}
