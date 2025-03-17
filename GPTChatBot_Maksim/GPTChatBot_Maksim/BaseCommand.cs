namespace Commands
{
    public abstract class BaseCommand(TelegramBotClient client)
    {
        protected readonly TelegramBotClient bot_client = client;
        protected readonly Logger logger = Logger.INSTANCE;

        public abstract string CommandName { get; }
    }

    public abstract class TextCommand(TelegramBotClient client) : BaseCommand(client)
    {
        public abstract Task ExecuteAsync(Update update);
    }

    public abstract class CallbackCommand(TelegramBotClient client) : BaseCommand(client)
    {
        public abstract Task ExecuteAsync(CallbackQuery callback);
    }
}
