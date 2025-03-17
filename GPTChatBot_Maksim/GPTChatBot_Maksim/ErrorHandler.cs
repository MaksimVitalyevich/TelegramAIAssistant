namespace Handlers
{
    /// <summary>
    /// Обработчик ошибок только API бот-клиента
    /// </summary>
    public class ErrorHandler
    {
        public ErrorHandler() { }

        /// <summary>
        /// Обработка API Ошибок бота
        /// </summary>
        /// <param name="exception">Исключение.</param>
        /// <returns>Логированная ошибка из пойманного исключения.</returns>
        public Task HandleError(Exception exception)
        {
            return Task.Run(() =>
            {
                string error_message = exception switch
                {
                    ApiRequestException apiReqEx => $"Ошибка API:\n{apiReqEx.ErrorCode} - {apiReqEx.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(error_message);
                Logger.INSTANCE.LogMessage(LogLevel.Error, error_message);
            });
        }
    }
}
