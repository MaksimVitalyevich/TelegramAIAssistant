using Assistant;
using Commands;
using Handlers;
using Utilities;

namespace TelegrammBot
{
    /// <summary>
    /// Основная "оболочка" Телеграмм-Бота. Здесь обрабатывается его: создание, запуск, отчеты об ошибках
    /// </summary>
   public class AI_TeleBot
    {
        private readonly TelegramBotClient bot_client;
        private readonly CommandManager commandManager;
        private readonly UpdateHandler handler_update;
        private readonly MessageHandler handler_message;
        private readonly FileHandler handler_file;
        private readonly CallBackHandler handler_callback;
        private readonly ErrorHandler handler_errors;
        private static AI_AssistantLogic ai_assistant;

        public static readonly ChatHistoryManager chatHistoryManager = new();
        public static readonly UserProfile userProfile = new();
        public static readonly Dictionary<long, bool> isAssistant_Mode = [];

        public static AI_AssistantLogic AI_ASSISTANT { get => ai_assistant; set => ai_assistant = value; }

        /// <summary>
        /// Создание Телеграмм-Бота. Токен бота, создается 2 способами и обрабатывается исключение при неудаче
        /// </summary>
        /// <param name="logDir"></param>
        /// <param name="tokenNullException"></param>
        public AI_TeleBot(Exception tokenNullException)
        {
            string token = Environment.GetEnvironmentVariable("GPTMaksim") ??
                ConfigurationManager.ConnectionStrings["GPTMaksim"]?.ConnectionString ?? 
                throw tokenNullException;

            string AiKey = Environment.GetEnvironmentVariable("DeepInfra") ??
                ConfigurationManager.ConnectionStrings["DeepInfra AIKey"]?.ConnectionString ??
                throw tokenNullException;

            bot_client = new(token);
            commandManager = new(bot_client, ai_assistant);
            handler_update = RegisterHandlers(handler_message, handler_callback, handler_file, handler_errors);
            AI_ASSISTANT = new(AiKey);
        }

        private void StartReceiving(Action<Exception> errorHandler)
        {
            bot_client.StartReceiving(async (client, update, token) => await handler_update.HandleUpdate(update),
                async (client, exception, token) => errorHandler(exception));
        }

        private UpdateHandler RegisterHandlers(MessageHandler msgHnd, CallBackHandler cbHnd, FileHandler fileHnd, ErrorHandler errHnd)
        {
            msgHnd = new(bot_client, ai_assistant, commandManager);
            fileHnd = new(bot_client, ai_assistant);
            cbHnd = new(bot_client, commandManager);
            errHnd = new();

            return new(msgHnd, cbHnd, fileHnd, errHnd);
        }

        public async Task StartAsync()
        {
            var me = await bot_client.GetMe();
            Console.WriteLine($"Бот {me.Username} запущен...");
            Logger.INSTANCE.LogMessage(LogLevel.Info, $"Бот `{me.Username}` запущен!");
            
            var reciever_options = new ReceiverOptions
            {
                AllowedUpdates = [],
                DropPendingUpdates = true
            };
            StartReceiving(exception => handler_errors.HandleError(exception));
            await Task.Delay(Timeout.Infinite); // Бесконечное ожидание не дающее программе завершится
        }
        
    }
}
