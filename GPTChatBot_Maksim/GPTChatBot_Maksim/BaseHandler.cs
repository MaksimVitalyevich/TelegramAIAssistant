namespace Handlers
{
    /// <summary>
    /// Базовый обработчик для определения записей логов и бот-клиента. При необходимости, наследуется
    /// </summary>
    public abstract class BaseHandler
    {
        protected readonly TelegramBotClient bot_client;
        protected readonly Logger logger = Logger.INSTANCE;

        public BaseHandler(TelegramBotClient client) => bot_client = client;
    }
}
