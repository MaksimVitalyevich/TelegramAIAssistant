using System;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using LoggerLibrary;
using static GPTChatBot_Maksim.ReplyDictionaries;
using System.Globalization;

namespace GPTChatBot_Maksim
{
    public class AI_TeleBot
    {
        private readonly TelegramBotClient bot_client;
        private readonly AI_AssistantLogic ai_assistant;

        private readonly string gpt_bot_welcome = "Приветствую! Я GPTAI-Бот Maksim. Главное меню:";
        private Message? text_message;
        private Chat? bot_chat;

        private readonly Dictionary<long, bool> isAssistant_Mode = [];
        private readonly Dictionary<long, List<string>> chat_histories = [];
        private readonly Dictionary<long, UserProfile> user_profiles = [];

        private readonly Random random = new();
        private readonly Logger logger;

        public AI_TeleBot(string logDir, Exception tokenNullException)
        {
            string token = Environment.GetEnvironmentVariable("GPTMaksim") ??
                ConfigurationManager.ConnectionStrings["GPTMaksim"]?.ConnectionString ?? 
                throw tokenNullException;

            string openAiKey = Environment.GetEnvironmentVariable("GPT AIKey") ??
                ConfigurationManager.ConnectionStrings["GPT AIKey"]?.ConnectionString ??
                throw tokenNullException;

            bot_client = new(token);
            ai_assistant = new(openAiKey);
            logger = new Logger(logDir, LogLevel.Error);
        }
        public async Task StartAsync()
        {
            var me = await bot_client.GetMe();
            Console.WriteLine($"Бот {me.Username} запущен...");
            logger.LogMessage(LogLevel.Info, $"Бот `{me.Username}` запущен!");
            
            var reciever_options = new ReceiverOptions
            {
                AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
                DropPendingUpdates = true
            };
            var CTS = new CancellationTokenSource();

            bot_client.StartReceiving(UpdateHandler, ErrorHandler, reciever_options, CTS.Token);
        }
        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            try
            {
                if (update.Message is { } message)
                {
                    bool handled = await OnCmdMenu(update) || await OnQuickAnswers(update);
                    if (!handled)
                        await OnMessageReplies(update);
                }
                else if (update.CallbackQuery is { } callbackQuery )
                {
                    await OnCallBackQuery(update);
                }
                else
                {
                    Console.WriteLine("Получен неизвестный тип сообщения.");
                    logger.LogMessage(LogLevel.Warning, "Получен неизвестный тип Update, не содержащий сообщения!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Попытка обработать некорректный запрос в UpdateHandler.");
                logger.LogMessage(LogLevel.Exception, $"Ошибка в обработке запроса UpdateHandler: {ex.Message}");
            }
        }

        private async Task<bool> OnCmdMenu(Update update)
        {
            text_message = update.Message ?? throw new ArgumentNullException(nameof(update.Message), "Сообщение пустое!");
            bot_chat = text_message.Chat ?? throw new ArgumentNullException(nameof(text_message.Chat), "Чат пустой!");
            var my_user = text_message.From ?? throw new ArgumentNullException(nameof(text_message.From), "пользователь не определен!");
            Console.WriteLine($"{my_user.FirstName} {my_user.LastName} Написал: {text_message.Text}");

            switch (text_message.Text)
            {
                case "/start":
                    await bot_client.SendMessage(bot_chat.Id, gpt_bot_welcome, replyMarkup: GetInlineKeyboard());
                    await bot_client.SendMessage(bot_chat.Id, "Нужна помощь с командами? Воспользуйтесь /help!");
                    return true;
                case "/inline_menu":
                    await bot_client.SendMessage(bot_chat.Id, gpt_bot_welcome, replyMarkup: GetInlineKeyboard());
                    return true;
                case "/assistant_mode":
                    if (!isAssistant_Mode.ContainsKey(bot_chat.Id) || !isAssistant_Mode[bot_chat.Id])
                    {
                        isAssistant_Mode[bot_chat.Id] = true;
                        await bot_client.SendMessage(bot_chat.Id, "*AI Режим - активирован!*" +
                            "\nТеперь ты можешь поговорить со мной на любые темы!", ParseMode.Markdown);
                    }
                    else
                    {
                        isAssistant_Mode[bot_chat.Id] = false;
                        await bot_client.SendMessage(bot_chat.Id, "*Режим AI - выключен!*" +
                            "\nТеперь я простой бот-помощник, выполняющие команды.", ParseMode.Markdown);
                    }
                    return true;
                case "/help":
                    string helpMsg = "*Доступные команды:*\n\n" +
                    "`/start` - Запуск бота\n" +
                    "`/inline_menu` - Вызов интерактивного меню\n" +
                    "`/assistant_mode` - Включает режим AI помощника для беседы\n" +
                    "`/quick_answers` - Меню быстрого набора ответов\n" +
                    "`/chat_settings` - Настройки для чата\n" +
                    "`/profile` - Просмотр профиля\n" +
                    "`/datetime` - Выводит текущее время и дату\n";
                    await bot_client.SendMessage(bot_chat.Id, helpMsg, ParseMode.Markdown);
                    return true;
                case "/datetime":
                    await bot_client.SendMessage(bot_chat.Id, $"Текущая Дата/Время: {DateTime.Now: dd:MM:yyyy HH:mm:ss}");
                    return true;
                case "/profile":
                    if (!user_profiles.TryGetValue(bot_chat.Id, out UserProfile? myProfile))
                    {
                        // Если нет профиля - отправляем сообщение и выходим из метода
                        await bot_client.SendMessage(bot_chat.Id, "Профиль не найден! Нужна регистрация.");
                        return false;
                    }
                    string profileInfo = $"*Профиль*\n" +
                    $"*Имя* {myProfile.Name}\n" +
                    $"*Любимая тема общения* {myProfile.FavouriteTopic}\n" +
                    $"*Стиль общения* {myProfile.ChatStyle}\n\n";
                    await bot_client.SendMessage(bot_chat.Id, profileInfo, ParseMode.Markdown);
                    return true;
                default:
                    return false;
            }
        }
        private static InlineKeyboardMarkup GetInlineKeyboard() => new(
        [
            [InlineKeyboardButton.WithCallbackData("Меню сайтов", "websites")],
            [InlineKeyboardButton.WithCallbackData("Быстрый набор реплик", "quick_answers")],
            [InlineKeyboardButton.WithCallbackData("Камень, Ножницы, Бумага...", "rps_minigame")],
            [InlineKeyboardButton.WithCallbackData("Настройки чата", "chat_settings")],
            [InlineKeyboardButton.WithCallbackData("Изменить профиль", "profile_change")]
        ]);
        private static InlineKeyboardMarkup GetRockPaperScissorsMenu() => new(
        [
            [InlineKeyboardButton.WithCallbackData("Камень", "rps_Камень")],
            [InlineKeyboardButton.WithCallbackData("Ножницы", "rps_Ножницы")],
            [InlineKeyboardButton.WithCallbackData("Бумага", "rps_Бумага")],
            [InlineKeyboardButton.WithCallbackData("Колодец", "rps_Колодец")],
        ]);
        private async Task OnCallBackQuery(Update update)
        {
            var callBackQuery = update.CallbackQuery ?? throw new ArgumentNullException(nameof(update.CallbackQuery), "Запрос неверен");
            bot_chat = callBackQuery.Message.Chat;

            await bot_client.AnswerCallbackQuery(callBackQuery.Id);
            Console.WriteLine($"Нажата {callBackQuery.Data}");
            logger.LogMessage(LogLevel.Info, $"Команда: {callBackQuery.Data}");

            switch (callBackQuery.Data)
            {
                case "back_to_main":
                    await bot_client.EditMessageText(bot_chat.Id, messageId: callBackQuery.Message.MessageId, gpt_bot_welcome, replyMarkup: GetInlineKeyboard());
                    break;
                case "websites":
                    await OnWebUrlOpen(callBackQuery);
                    break;
                case "quick_answers":
                    await OnQuickAnswers(new Update { Message = new Message { Text = "/quick_answers", Chat = bot_chat } });
                    break;
                case "rps_minigame":
                    await ShowRockPaperScissorsMenu(callBackQuery);
                    break;
                case "rps_Камень":
                case "rps_Ножницы":
                case "rps_Бумага":
                case "rps_Колодец":
                    await OnRockPaperScissors(callBackQuery);
                    break;
                case "chat_settings":
                    await OnChatSettings(callBackQuery);
                    break;
                case "load_chat":
                    string chat_history = LoadChatHistory(bot_chat.Id);
                    await bot_client.SendMessage(bot_chat.Id, $"История чата:*\n\n{chat_history}", ParseMode.Markdown);
                    break;
                case "clear_chat":
                    chat_histories.Remove(bot_chat.Id);
                    await bot_client.SendMessage(bot_chat.Id, "Твоя история чатов - очищена!");
                    break;
                case "profile_change":
                    await OnProfileCommand(callBackQuery);
                    break;
                default:
                    await bot_client.SendMessage(bot_chat.Id, Misunderstanding[random.Next(Misunderstanding.Count)]);
                    break;
            }
        }
        private async Task OnWebUrlOpen(CallbackQuery callbackQuery)
        {
            bot_chat = callbackQuery.Message.Chat;

            var websiteMenu = new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithUrl("Откуда я беру факты:", "https://ru.wikipedia.org")],
                [InlineKeyboardButton.WithUrl("Мой друг-помощник:", "https://chatgpt.com/")],
                [InlineKeyboardButton.WithUrl("Актуальные забавные шутки", "https://www.geeksforgeeks.org/short-jokes/")],
                [InlineKeyboardButton.WithCallbackData("<- Назад", "back_to_main")]
            ]);
            await bot_client.EditMessageText(bot_chat.Id, messageId: callbackQuery.Message.MessageId, "Сайты:", replyMarkup: websiteMenu);
        }
        private static ReplyKeyboardMarkup GetReplyKeyboard() => new(
        [
            [new("Привет, Бот!"), new("Как дела?")],
            [new("Выдать случайное число"), new("Какой сегодня день?")],
            [new("Случайный факт!"), new("Выдать шутку")],
            [new("Подсказать Дату и Время")],
            [new("Прощай, Бот!")]
        ])
        { ResizeKeyboard = true, OneTimeKeyboard = false };
        private async Task<bool> OnQuickAnswers(Update update)
        {
            text_message = update.Message ?? throw new ArgumentNullException(nameof(update.Message), "Сообщение пустое");
            var userName = text_message.From;
            bot_chat = text_message.Chat;
            var replyKeyboard = GetReplyKeyboard();
            
            if (text_message.Text == "/quick_answers")
            {
                await bot_client.SendMessage(bot_chat.Id, "Меню быстрых ответов:", replyMarkup: replyKeyboard);
                return true;
            }

            string quick_response = text_message.Text switch
            {
                "Привет, Бот!" => Greetings[random.Next(Greetings.Length)] + $"{userName.FirstName}!",
                "Как дела?" => Feelings[random.Next(Feelings.Length)],
                "Выдать случайное число" => $"Твое число будет: {random.Next(1, 100)}",
                "Какой сегодня день?" => $"Сегодня {DateTime.Now.ToString("dddd", new CultureInfo("ru-RU"))}",
                "Случайный факт!" => $"{Facts[random.Next(Facts.Length)]}",
                "Выдать шутку" => $"{Jokes[random.Next(Jokes.Length)]}",
                "Подсказать Дату и Время" => $"Текущая Дата/Время: {DateTime.Now: dd:MM:yyyy HH:mm:ss}",
                "Прощай, Бот!" => Goodbyes[random.Next(Goodbyes.Length)] + $"{userName.FirstName}!",
                _ => ""
            };

            if (quick_response is null)
                return false;

            await bot_client.SendMessage(bot_chat.Id, quick_response, replyMarkup: replyKeyboard);
            SaveMessage(bot_chat.Id, text_message.Text ?? throw new ArgumentNullException(nameof(text_message.Text), "Сообщение пустое"));
            return true;
        }
        private async Task ShowRockPaperScissorsMenu(CallbackQuery callbackQuery)
        {
            bot_chat = callbackQuery.Message.Chat;
            await bot_client.EditMessageText(bot_chat.Id, messageId: callbackQuery.Message.MessageId, "*Ваш ход:*", ParseMode.Markdown, replyMarkup: GetRockPaperScissorsMenu());
        }
        private async Task OnRockPaperScissors(CallbackQuery callbackQuery)
        {
            string[] rps_choices = { "Камень", "Ножницы", "Бумага", "Колодец" };
            string user_rps_choice = callbackQuery.Data.Replace("rps_", "");
            string bot_rps_choice = rps_choices[random.Next(rps_choices.Length)];
            string resultMsg;

            if (user_rps_choice == bot_rps_choice)
            {
                resultMsg = $"Ничья! *{user_rps_choice} - {bot_rps_choice}*";
            }
            else if ((user_rps_choice == "Камень" && (bot_rps_choice == "Ножницы" || bot_rps_choice == "Колодец")) ||
                (user_rps_choice == "Ножницы" && bot_rps_choice == "Бумага") ||
                (user_rps_choice == "Бумага" && bot_rps_choice == "Камень") ||
                (user_rps_choice == "Колодец" && bot_rps_choice == "Камень"))
            {
                resultMsg = $"Победа! *{user_rps_choice}* > *{bot_rps_choice}*";
            }
            else
            {
                resultMsg = $"Проигрыш! *{user_rps_choice}* < *{bot_rps_choice}*";
            }
            await bot_client.EditMessageText(bot_chat.Id, messageId: callbackQuery.Message.MessageId, resultMsg, ParseMode.Markdown);
            await Task.Delay(2500);
            await bot_client.EditMessageText(bot_chat.Id, messageId: callbackQuery.Message.MessageId, gpt_bot_welcome, replyMarkup: GetInlineKeyboard());
        }
        private async Task<bool> OnChatSettings(CallbackQuery callbackQuery)
        {
            bot_chat = callbackQuery.Message.Chat;

            var chatMenu = new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("Новый чат", "new_chat")],
                [InlineKeyboardButton.WithCallbackData("Загрузить чат", "load_chat")],
                [InlineKeyboardButton.WithCallbackData("Очистить чат", "clear_chat")],
                [InlineKeyboardButton.WithCallbackData("Удалить чат", "delete_chat")],
                [InlineKeyboardButton.WithCallbackData("<- Назад", "back_to_main")]
            ]);
            
            await bot_client.EditMessageText(bot_chat.Id, messageId: callbackQuery.Message.MessageId, "Меню чата:", replyMarkup: chatMenu);
            return true;
        }
        private async Task OnMessageReplies(Update update)
        {
            text_message = update.Message ?? throw new ArgumentNullException(nameof(update.Message), "Сообщение пустое!");
            bot_chat = text_message.Chat;

            if (!isAssistant_Mode.ContainsKey(bot_chat.Id) || !isAssistant_Mode[bot_chat.Id])
            {
                await bot_client.SendMessage(bot_chat.Id, "Для возможности поболтать с настоящим AI-ботом, включите режим ассистента командой " +
                    "`/assistant_mode`!", ParseMode.Markdown);
                return;
            }

            if (string.IsNullOrEmpty(text_message.Text))
            {
                Console.WriteLine("Попытка отправить пустое сообщение для AI-разговора!");
                logger.LogMessage(LogLevel.Warning, "Пользователь отправил пустое сообщение. AI-ответ не запрашивается.");
                return;
            }

            string aiResponse = await ai_assistant.GenerateResponse(text_message.Text);
            await bot_client.SendMessage(bot_chat.Id, aiResponse);
            SaveMessage(bot_chat.Id, text_message.Text);
        }
        private void SaveMessage(long chatID, string message)
        {
            if (!chat_histories.ContainsKey(chatID))
                chat_histories[chatID] = [];
            chat_histories[chatID].Add(message);
            logger.LogMessage(LogLevel.Info, message);
        }
        private string LoadChatHistory(long chatID)
        {
            return chat_histories.TryGetValue(chatID, out List<string>? archive_message) ? string.Join("\n", archive_message) : "История чата пуста.";
        }
        private async Task<bool> OnProfileCommand(CallbackQuery callbackQuery)
        {
            bot_chat = callbackQuery.Message.Chat;
            var userId = callbackQuery.From.Id;

            if (!user_profiles.ContainsKey(userId))
                user_profiles[userId] = new UserProfile();

            var profileMenu = new InlineKeyboardMarkup(
            [
                [InlineKeyboardButton.WithCallbackData("Изменить имя", "set_name ")],
                [InlineKeyboardButton.WithCallbackData("Изменить тему", "set_topic ")],
                [InlineKeyboardButton.WithCallbackData("Изменить стиль общения", "set_style ")],
                [InlineKeyboardButton.WithCallbackData("<- Назад", "back_to_main")]
            ]);

            string profileSettings = $"Для изменения профиля Бота, используй: `/set_name Имя`, `/set_topic Тема`, `/set_style Стиль`.";
            await bot_client.EditMessageText(bot_chat.Id, messageId: callbackQuery.Message.MessageId, profileSettings, ParseMode.Markdown, replyMarkup: profileMenu);
            return true;
        }
        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
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
