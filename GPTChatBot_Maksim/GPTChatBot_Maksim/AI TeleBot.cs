namespace GPTChatBot_Maksim
{
    /// <summary>
    /// Основная "оболочка" Телеграмм-Бота. Здесь обрабатывается его: создание, запуск, отчеты об ошибках
    /// </summary>
    public class AI_TeleBot
    {
        private readonly TelegramBotClient bot_client;
        private readonly CommandManager commandManager;
        public static AI_AssistantLogic ai_assistant;
        /// <summary>
        /// Приветствие Телеграмм-Бота, отправляется плавно по частям
        /// </summary>
        public static string GPTBotWelcome { get; private set; } = "Приветствую! Я *GPTMaksim*\nПростой бот для разговора с AI возможностями!\n" +
            "Умею:\n 🔹 Отвечать на ваши вопросы;\n 🔹 Обрабатывать файлы;\n 🔹 Предлагать полезные факты или смешно пошутить;\n" +
            " 🔹 Работать с чатами;\n 🔹 Работать с профилем пользователя;\n 🔹 Ну и просто провести с тобой хорошее время!";
        public static readonly Dictionary<long, bool> isAssistant_Mode = [];
        public static readonly Dictionary<long, DateTime> lastcmdTime = [];
        private readonly Logger logger;

        /// <summary>
        /// Создание Телеграмм-Бота. Токен бота, создается 2 способами и обрабатывается исключение при неудаче
        /// </summary>
        /// <param name="logDir"></param>
        /// <param name="tokenNullException"></param>
        public AI_TeleBot(string logDir, Exception tokenNullException)
        {
            string token = Environment.GetEnvironmentVariable("GPTMaksim") ??
                ConfigurationManager.ConnectionStrings["GPTMaksim"]?.ConnectionString ?? 
                throw tokenNullException;

            string openAiKey = Environment.GetEnvironmentVariable("GPT AIKey") ??
                ConfigurationManager.ConnectionStrings["GPT AIKey"]?.ConnectionString ??
                throw tokenNullException;

            bot_client = new(token);
            commandManager = new(bot_client);
            ai_assistant = new(openAiKey);
            logger = new Logger(logDir, LogLevel.Error);
        }
        /// <summary>
        /// Запуск работы бота в бесконечном цикле
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            var me = await bot_client.GetMe();
            Console.WriteLine($"Бот {me.Username} запущен...");
            logger.LogMessage(LogLevel.Info, $"Бот `{me.Username}` запущен!");
            
            var reciever_options = new ReceiverOptions
            {
                AllowedUpdates = [],
                DropPendingUpdates = true
            };
            bot_client.StartReceiving(updateHandler: async (client, update, token) => await commandManager.HandleUpdate(update),
                errorHandler: async (client, exception, token) => await ErrorHandler(exception), reciever_options);
        }
        /// <summary>
        /// Обработчик API ошибок бота
        /// </summary>
        /// <param name="exception">API Исключение.</param>
        /// <returns></returns>
        private async Task ErrorHandler(Exception exception)
        {
            string error_message = exception switch
            {
                ApiRequestException apiReqEx => $"Ошибка API:\n{apiReqEx.ErrorCode} - {apiReqEx.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(error_message);
            logger.LogMessage(LogLevel.Error, error_message);
            await Task.CompletedTask;
        }
    }
}
