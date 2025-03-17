namespace Handlers
{
    /// <summary>
    /// Общий обработчик чат-бота. Управляет разными типами обновлении/запросов
    /// </summary>
    public class UpdateHandler(MessageHandler msgHnd, CallBackHandler cbHnd, FileHandler fileHnd, ErrorHandler errHnd)
    {
        private readonly MessageHandler messageHandler = msgHnd;
        private readonly CallBackHandler callbackHandler = cbHnd;
        private readonly FileHandler fileHandler = fileHnd;
        private readonly ErrorHandler errorHandler = errHnd;

        /// <summary>
        /// Общая обработка обновлении
        /// </summary>
        /// <param name="update">Тип обновления.</param>
        /// <returns>Определенный ответ, в зависимости от поступаемого типа. При неудаче, вызывает ошибку.</returns>
        public async Task HandleUpdate(Update update)
        {
            try
            {
                if (update.Message != null)
                {
                    await fileHandler.HandleFile(update);
                    await messageHandler.HandleUpdate(update);
                }
                else if (update.CallbackQuery is { } callback)
                {
                    await callbackHandler.HandleCallback(callback);
                }
            }
            catch (Exception ex)
            {
                await errorHandler.HandleError(ex);
            }
        }
    }
}
