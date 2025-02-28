using GPTChatBot_Maksim.Commands;
using GPTChatBot_Maksim.Utilities;

namespace GPTChatBot_Maksim
{
    /// <summary>
    /// Интерфейс для обработки всех типов команд Телеграмм-Бота
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Имя для команды
        /// </summary>
        string? CommandName { get; }
        /// <summary>
        /// Асинхронное выполнение команды с типом Update. По умолчанию, возвращает пустую выполненную задачу
        /// </summary>
        /// <param name="update">Параметр обновления.</param>
        /// <returns></returns>
        async Task ExecuteAsync(Update update) => await Task.CompletedTask;
        /// <summary>
        /// Асинхронное выполнение команды с типом CallbackQuery. По умолчанию, возвращает пустую выполненную задачу
        /// </summary>
        /// <param name="callback">Параметр запроса.</param>
        /// <returns></returns>
        async Task ExecuteAsync(CallbackQuery callback) => await Task.CompletedTask;
    }
    /// <summary>
    /// Менеджер команд бота. Обрабатывает все типы команд
    /// </summary>
    internal class CommandManager
    {
        private readonly TelegramBotClient bot_client;
        private readonly List<ICommand> commands;
        private readonly ChatHistoryManager historyManager;
        public static readonly UserProfile userProfile = new();
        private readonly Logger logger = new("..\\..\\..\\MaksimBotExceptions", LogLevel.Exception);
        /// <summary>
        /// Конструктор для списка всех команд-классов. Подключен логгер и менеджер управления чатами
        /// </summary>
        /// <param name="client"></param>
        /// <param name="aiAssistant"></param>
        public CommandManager(TelegramBotClient client)
        {
            bot_client = client;
            historyManager = new(logger);
            // [] - эквивалетно для commands = new List<ICommand> {};
            commands =
            [
                new OnStart(bot_client),
                new OnHelp(bot_client),
                new OnInlineMenu(bot_client),
                new OnAssistantMenuShow(bot_client),
                new OnAssistantOn(bot_client),
                new OnAssistantOff(bot_client),
                new OnWebUrlOpen(bot_client),
                new OnQuickReply(bot_client, historyManager),
                new OnMessageReply(bot_client, historyManager),
                new OnChatSettings(bot_client),
                new OnNewChat(bot_client, historyManager),
                new OnSelectChat(bot_client, historyManager),
                new OnLoadChat(bot_client, historyManager),
                new OnDeleteChat(bot_client, historyManager),
                new OnRPSMenuShow(bot_client),
                new OnRPSGameStart(bot_client),
                new OnDateTimePrint(bot_client),
                new OnProfileShow(bot_client),
                new OnProfileChange(bot_client),
                new OnProfileNameSet(bot_client),
                new OnProfileTopicSet(bot_client),
                new OnProfileStyleSet(bot_client),
                new OnBackToMain(bot_client)
            ];
        }
        /// <summary>
        /// Обработчик (делегат) команд с универсальной проверкой, запускается из StartHandleUpdate
        /// </summary>
        /// <param name="update">Тип обновления.</param>
        /// <returns></returns>
        public async Task HandleUpdate(Update update)
        {
            Console.WriteLine($"Получено обновление типа: {update.Type}");

            try
            {
                // Обработка ТОЛЬКО текстовых команд, либо быстрых ответов сообщении и сообщении AI
                if (update.Message?.Text is { } messageText)
                {
                    logger.LogMessage(LogLevel.Info, $"[DEBUG] Текст сообщения: {messageText}");

                    var command = commands.FirstOrDefault(c => c.CommandName == messageText);
                    if (command != null)
                    {
                        Console.WriteLine($"[DEBUG] Найдена команда: {command.CommandName}");
                        await command.ExecuteAsync(update); // Обработка текстовых команд
                    }
                    else if (commands.OfType<OnQuickReply>().Any(q => q.CommandName == "/quick_answers" && IsQuickReply(messageText)))
                    {
                        // Если сообщение - это быстрый ответ
                        var quickReplyCommand = commands.OfType<OnQuickReply>().FirstOrDefault();
                        if (quickReplyCommand != null)
                            await quickReplyCommand.ExecuteAsync(update);
                    }
                    else
                    {
                        // Если сообщение не является быстрым ответом или текстовой командой, то генерируем AI-ответ
                        var replyCommand = commands.OfType<OnMessageReply>().FirstOrDefault();
                        if (replyCommand != null)
                            await replyCommand.ExecuteAsync(update);
                    }
                }
                // Обработка ТОЛЬКО запросов
                else if (update.CallbackQuery is { } callback)
                {
                    logger.LogMessage(LogLevel.Info, $"[DEBUG] Нажата кнопка: {callback.Data}");

                    var command = commands.FirstOrDefault(c => callback.Data.StartsWith(c.CommandName));
                    if (command != null)
                    {
                        Console.WriteLine($"[DEBUG] Найдена команда: {command.CommandName}");
                        await command.ExecuteAsync(callback); // Обработка запросов
                    }
                    else if (callback.Data == "quick_answers")
                    {
                        // если была нажата кнопка вызова меню быстрых реплик
                        var quickreplyCommand = commands.OfType<OnQuickReply>().FirstOrDefault();
                        if (quickreplyCommand != null)
                            await quickreplyCommand.ExecuteAsync(new Update { Message = new Message { Text = "/quick_answers", Chat = callback.Message.Chat } });
                    }
                }
                else
                {
                    Console.WriteLine("[ERROR] Неизвестная команда!");
                    logger.LogMessage(LogLevel.Warning, "Получен неизвестный тип запроса, без сообщения!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HandleUpdate - Не получилось обработать запрос/сообщение.");
                logger.LogMessage(LogLevel.Exception, $"Ошибка работы Телеграмм-Бота: {ex.Message}");
            }
        }
        /// <summary>
        /// Метод проверки, нужно ли обрабатывать кнопки быстрых ответов, через запрос?
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <returns></returns>
        private bool IsQuickReply(string message)
        {
            return message switch
            {
                "Привет, Бот!" => true,
                "Как дела?" => true,
                "Выдать случайное число" => true,
                "Какой сегодня день?" => true,
                "Случайный факт!" => true,
                "Выдать шутку" => true,
                "Подсказать Дату и Время" => true,
                "Прощай, Бот!" => true,
                _ => false
            };
        }
    }
}
