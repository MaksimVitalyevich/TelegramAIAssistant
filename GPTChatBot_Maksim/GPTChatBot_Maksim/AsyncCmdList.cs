using static GPTChatBot_Maksim.Dictionaries.ReplyDictionaries;
using GPTChatBot_Maksim.Utilities;
using static GPTChatBot_Maksim.Utilities.KeyboardGetter;

namespace GPTChatBot_Maksim.Commands
{
    public class OnStart : ICommand
    {
        public string CommandName => "/start";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда запуска бота
        /// </summary>
        /// <param name="client"></param>
        public OnStart(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message.Chat.Id;
            var commandCheck = update.Message.Text;
            string[] msg_parts = AI_TeleBot.GPTBotWelcome.Split("\n");

            if ((commandCheck == CommandName) && AI_TeleBot.lastcmdTime.TryGetValue(chatID, out DateTime sinceLastTime))
            {
                if ((DateTime.Now - sinceLastTime).TotalSeconds < 3)
                {
                    await bot_client.SendMessage(chatID, "Подожди немного, прежде чем вызвать повторно данную команду!");
                    return;
                }
            }
            AI_TeleBot.lastcmdTime[chatID] = DateTime.Now;

            foreach (var part in msg_parts)
            {
                await bot_client.SendChatAction(chatID, ChatAction.Typing);
                await Task.Delay(300);
                await bot_client.SendMessage(chatID, part, ParseMode.Markdown);
            }

            await Task.Delay(600);
            await bot_client.SendMessage(chatID, "Вот мое главное меню, выбирай что хочешь:", replyMarkup: GetInlineKeyboard());
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1500);
            await bot_client.SendMessage(chatID, "Нужна помощь с командами? Воспользуйтесь /help!");
        }
    }
    public class OnHelp : ICommand
    {
        public string CommandName => "/help";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда вызова помощи "Что умеет бот?"
        /// </summary>
        /// <param name="client"></param>
        public OnHelp(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message.Chat.Id;
            var commandCheck = update.Message.Text;

            if ((commandCheck == CommandName) && AI_TeleBot.lastcmdTime.TryGetValue(chatID, out DateTime sinceLastTime))
            {
                if ((DateTime.Now - sinceLastTime).TotalSeconds < 3)
                {
                    await bot_client.SendMessage(chatID, "Подожди немного, прежде чем вызвать повторно данную команду!");
                    return;
                }
            }
            AI_TeleBot.lastcmdTime[chatID] = DateTime.Now;

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(5500);
            string helpMsg = "*Доступные команды:*\n\n" +
                "🔹 `/start` - Запуск бота\n" +
                "🔹 `/inline_menu` - Вызов интерактивного меню\n" +
                "🔹 `/quick_answers` - Меню быстрого набора ответов\n" +
                "🔹 `/profile` - Просмотр профиля\n" +
                "🔹 `/datetime` - Выводит текущее время и дату\n\n" +
                "*Кнопки меню (нажми их через Inline-меню выше):*\n" +
                " *Меню сайтов* - ссылки полезных источников (и не очень)\n" +
                " *Быстрый набор реплик* - Вызов Reply-меню ответов-заготовок\n" +
                " *Режим ассистента* - Включение AI режима для бота\n" +
                " *КНБ мини-игра* - Мини-игра в Камень, Ножницы, Бумага\n" +
                " *Настройки чата* - Изменение параметров чата\n" +
                " *Изменить Профиль* - Изменение параметров профиля пользователя\n\n" +
                "Чтобы увидеть такие кнопки снова, вызови команду `/inline_menu`.";
            await bot_client.SendMessage(chatID, helpMsg, ParseMode.Markdown);
        }
    }
    public class OnInlineMenu : ICommand
    {
        public string CommandName => "/inline_menu";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда повторного вызова главного меню бота
        /// </summary>
        /// <param name="client"></param>
        public OnInlineMenu(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message.Chat.Id;
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(300);
            await bot_client.SendMessage(chatID, "Вот мое главное меню, выбирай что хочешь:", ParseMode.Markdown, replyMarkup: GetInlineKeyboard());
        }
    }
    public class OnAssistantMenuShow : ICommand
    {
        public string CommandName => "assistant_mode";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда вложенного меню для настройки режима AI-ассистента
        /// </summary>
        /// <param name="client"></param>
        public OnAssistantMenuShow(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;

            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "Режим ассистента:", replyMarkup: GetAssistantKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnAssistantOn : ICommand
    {
        public string CommandName => "assistant_on";
        private readonly TelegramBotClient bot_client;
        public OnAssistantOn(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;

            // Проверка, не включен ли заранее режим "Ассистент AI"
            if (AI_TeleBot.isAssistant_Mode.TryGetValue(chatID, out bool IsEnabled) && IsEnabled)
            {
                await bot_client.AnswerCallbackQuery(callback.Id, "AI режим - уже активирован!", showAlert: false);
                return;
            }

            AI_TeleBot.isAssistant_Mode[chatID] = true;

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1600);
            await bot_client.SendMessage(chatID, "*AI Режим - активирован!*" +
                "\nТеперь ты можешь поговорить со мной на любые темы!", ParseMode.Markdown);

            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnAssistantOff : ICommand
    {
        public string CommandName => "assistant_off";
        private readonly TelegramBotClient bot_client;
        public OnAssistantOff(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;

            // Проверка, что режим AI уже выключен
            if (AI_TeleBot.isAssistant_Mode.TryGetValue(chatID, out bool IsEnabled) && IsEnabled)
            {
                await bot_client.AnswerCallbackQuery(callback.Id, "AI режим - уже выключен!", showAlert: false);
                return;
            }

            AI_TeleBot.isAssistant_Mode[chatID] = false;

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1600);
            await bot_client.SendMessage(chatID, "*Режим AI - выключен!*" +
                "\nТеперь я простой бот-помощник, выполняющие команды.", ParseMode.Markdown);

            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnWebUrlOpen : ICommand
    {
        public string CommandName => "websites";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда вложенного меню "Сайты"
        /// </summary>
        /// <param name="client"></param>
        public OnWebUrlOpen(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "Сайты:", replyMarkup: GetWebsiteMenuKeyboard());
        }
    }
    public class OnBackToMain : ICommand
    {
        public string CommandName => "back_to_main";
        private readonly TelegramBotClient bot_client;
        private readonly string gpt_bot_welcome = "Приветствую! Я *GPTAI-Бот Maksim*. Главное меню:";
        /// <summary>
        /// Команда-кнопка для возврата в главное меню бота
        /// </summary>
        /// <param name="client"></param>
        public OnBackToMain(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, gpt_bot_welcome, ParseMode.Markdown, replyMarkup: GetInlineKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnQuickReply : ICommand
    {
        public string CommandName => "/quick_answers";
        private readonly TelegramBotClient bot_client;
        private readonly ChatHistoryManager history_manager;
        private readonly Random random = new();
        /// <summary>
        /// Команда вложенного меню для обработки быстрык заготовленных реплик. Выполняется как текстовая команда /quick_answers
        /// </summary>
        /// <param name="client"></param>
        /// <param name="history"></param>
        public OnQuickReply(TelegramBotClient client, ChatHistoryManager history)
        {
            bot_client = client;
            history_manager = history;
        }
        public async Task ExecuteAsync(Update update)
        {
            var userName = update.Message.From;
            var chatID = update.Message.Chat.Id;

            if (update.Message.Text == "/quick_answers")
            {
                await bot_client.SendChatAction(chatID, ChatAction.Typing);
                await Task.Delay(1000);
                await bot_client.SendMessage(chatID, "Меню быстрых ответов:", replyMarkup: GetReplyKeyboard());
                return;
            }
            await ProcessQuickRepliesText(update.Message.Text, chatID, userName.FirstName);
            history_manager.SaveMessage(chatID, update.Message.Text);
        }
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            await bot_client.SendMessage(chatID, "Меню быстрых ответов:", replyMarkup: GetReplyKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
        private async Task ProcessQuickRepliesText(string userText, long chatID, string firstName)
        {
            string quick_response = userText switch
            {
                "Привет, Бот!" => Greetings[random.Next(Greetings.Length)] + $"{firstName}!" + "\nЖду твоих указании...",
                "Как дела?" => Feelings[random.Next(Feelings.Length)],
                "Выдать случайное число" => $"Твое число будет: {random.Next(1, 100)}",
                "Какой сегодня день?" => $"Сегодня {DateTime.Now.ToString("dddd", new CultureInfo("ru-RU"))}",
                "Случайный факт!" => $"Занятный факт: {Facts[random.Next(Facts.Length)]}",
                "Выдать шутку" => $"{Jokes[random.Next(Jokes.Length)]}",
                "Подсказать Дату и Время" => $"Текущая Дата/Время: {DateTime.Now: dd:MM:yyyy HH:mm:ss}",
                "Прощай, Бот!" => Goodbyes[random.Next(Goodbyes.Length)] + $"{firstName}!",
                _ => ""
            };

            if (quick_response is not null)
            {
                await bot_client.SendChatAction(chatID, ChatAction.Typing);
                await Task.Delay(1800);
                await bot_client.SendMessage(chatID, quick_response, replyMarkup: GetReplyKeyboard());
            }
        }
    }
    public class OnMessageReply : ICommand
    {
        public string CommandName => "message_reply"; // Напрямую не используется, но нужна для структуры 
        private readonly TelegramBotClient bot_client;
        private readonly ChatHistoryManager history_manager;
        private readonly Random random = new();
        /// <summary>
        /// Команда для генерации AI-ответов, только если был включен "Режим Ассистента"
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ai_assistant"></param>
        /// <param name="history"></param>
        public OnMessageReply(TelegramBotClient client, ChatHistoryManager history)
        {
            bot_client = client;
            history_manager = history;
        }
        public async Task ExecuteAsync(Update update)
        {
            if (update.Message?.Text is not { } userMsg) return;
            var chatID = update.Message.Chat.Id;
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(2000);
            await bot_client.SendMessage(chatID, Misunderstanding[random.Next(Misunderstanding.Count)]);

            if (!AI_TeleBot.isAssistant_Mode.ContainsKey(chatID) || !AI_TeleBot.isAssistant_Mode[chatID])
            {
                await bot_client.SendChatAction(chatID, ChatAction.Typing);
                await Task.Delay(3000);
                await bot_client.SendMessage(chatID, "Для возможности поболтать с настоящим AI-ботом, " +
                    "включите режим ассистента через вложенное inline-Меню *Режим ассистента*!", ParseMode.Markdown);
                return;
            }

            if (string.IsNullOrEmpty(userMsg))
            {
                Console.WriteLine("Попытка отправить пустое сообщение для AI-разговора!");
                return;
            }

            string aiResponse = await AI_TeleBot.ai_assistant.GenerateResponse(userMsg);
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1600);
            await bot_client.SendMessage(chatID, aiResponse);
            history_manager.SaveMessage(chatID, userMsg);
        }
    }
    public class OnChatSettings : ICommand
    {
        public string CommandName => "chat_settings";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда вложенного меню для настроек чата
        /// </summary>
        /// <param name="client"></param>
        public OnChatSettings(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "Меню чата:", replyMarkup: GetChatMenuKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnNewChat : ICommand
    {
        public string CommandName => "new_chat";
        private readonly TelegramBotClient bot_client;
        private readonly ChatHistoryManager history_manager;
        /// <summary>
        /// Команда-кнопка для создания новго чата по его ID
        /// </summary>
        /// <param name="client"></param>
        /// <param name="history"></param>
        public OnNewChat(TelegramBotClient client, ChatHistoryManager history)
        {
            bot_client = client;
            history_manager = history;
        }
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            string newChatID = history_manager.NewChatHistory(chatID);

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1500);
            await bot_client.SendMessage(chatID, $"Создан новый чат: *{newChatID}*.\nНовый чат - активирован!", ParseMode.Markdown);
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnSelectChat : ICommand
    {
        public string CommandName => "select_chat";
        private readonly TelegramBotClient bot_client;
        private readonly ChatHistoryManager history_manager;
        /// <summary>
        /// Команда-кнопка для выбора чата из списка <see cref="history_manager"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="history"></param>
        public OnSelectChat(TelegramBotClient client, ChatHistoryManager history)
        {
            bot_client = client;
            history_manager = history;
        }
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            var chatList = history_manager.GetChatList(chatID);

            if (chatList.Count == 0)
            {
                await bot_client.SendChatAction(chatID, ChatAction.Typing);
                await Task.Delay(500);
                await bot_client.SendMessage(chatID, "У тебя нет сохраненных чатов.");
                return;
            }
            var buttons = chatList.Select(name => InlineKeyboardButton.WithCallbackData($"[] {name}", $"switch_chat|{name}")).ToArray();
            var menu = new InlineKeyboardMarkup(buttons);

            await Task.Delay(1200);

            await bot_client.SendMessage(chatID, "Выбери чат:", replyMarkup: menu);
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnSwitchChat : ICommand
    {
        public string CommandName => "switch_chat";
        private readonly TelegramBotClient bot_client;
        private readonly ChatHistoryManager history_manager;
        /// <summary>
        /// Внутренняя команда-кнопка переключения между чатами. Не используется напрямую через Manager
        /// </summary>
        /// <param name="client"></param>
        /// <param name="history"></param>
        public OnSwitchChat(TelegramBotClient client, ChatHistoryManager history)
        {
            bot_client = client;
            history_manager = history;
        }
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            string chatName = callback.Data.Replace("switch_chat|", "");

            if (history_manager.SetActiveChat(chatID, chatName))
                await bot_client.SendMessage(chatID, $"Чат переключен на: *{chatName}*", ParseMode.Markdown);
            else
                await bot_client.SendMessage(chatID, "Ошибка: этот чат не найден.");

            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnLoadChat : ICommand
    {
        public string CommandName => "load_chat";
        private readonly TelegramBotClient bot_client;
        private readonly ChatHistoryManager history_manager;
        /// <summary>
        /// Команда-кнопка для загрузки выбранного чата из списка <see cref="history_manager"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="history"></param>
        public OnLoadChat(TelegramBotClient client, ChatHistoryManager history)
        {
            bot_client = client;
            history_manager = history;
        }
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            string chatHistory = history_manager.LoadChatHistory(chatID);
            await Task.Delay(1200);
            await bot_client.SendMessage(chatID, $"История чата:*\n\n{chatHistory}*", ParseMode.Markdown);
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnDeleteChat : ICommand
    {
        public string CommandName => "delete_chat";
        private readonly TelegramBotClient bot_client;
        private readonly ChatHistoryManager history_manager;
        /// <summary>
        /// Команда-кнопка для удаления выбранного чата из <see cref="history_manager"/>s"/>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="history"></param>
        public OnDeleteChat(TelegramBotClient client, ChatHistoryManager history)
        {
            bot_client = client;
            history_manager = history;
        }
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            string chatName = callback.Data.Replace("delete_chat_", "");

            await Task.Delay(1200);

            if (history_manager.DeleteChatFromHistory(chatID, chatName))
                await bot_client.SendMessage(chatID, $"Чат *{chatName}* удален!", ParseMode.Markdown);
            else
                await bot_client.SendMessage(chatID, "Такой чат не найден.");

            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnRPSMenuShow : ICommand
    {
        public string CommandName => "rps_minigame";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда вложенного меню мини-игры "Камень, Ножницы, Бумага..."
        /// </summary>
        /// <param name="client"></param>
        public OnRPSMenuShow(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            await Task.Delay(500);
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "*Ваш ход:*", ParseMode.Markdown, replyMarkup: GetRockPaperScissorsMenu());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
    public class OnRPSGameStart : ICommand
    {
        public string CommandName => "rps_"; // Не фиксированное имя, будет проверяться через CommandManager
        private readonly TelegramBotClient bot_client;
        private static readonly string[] rps_choices = ["Камень", "Ножницы", "Бумага", "Колодец"];
        private readonly Random random = new();
        /// <summary>
        /// Команда из кнопок для обработки выбора действии игрока <see cref="rps_choices"/>
        /// </summary>
        /// <param name="client"></param>
        public OnRPSGameStart(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            string user_rps_choice = callback.Data.Replace("rps_", ""); // Убираем префикс "rps_"
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

            var chatID = callback.Message.Chat.Id;

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1500);
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, resultMsg, ParseMode.Markdown);
            await Task.Delay(3000);
            await bot_client.SendChatAction(chatID, ChatAction.Typing);

            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "Вот мое главное меню, выбирай что хочешь:", replyMarkup: GetInlineKeyboard());
        }
    }
    public class OnDateTimePrint : ICommand
    {
        public string CommandName => "/datetime";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда для вывода текущей даты и времени
        /// </summary>
        /// <param name="client"></param>
        public OnDateTimePrint(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message.Chat.Id;
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(2500);
            await bot_client.EditMessageText(chatID, messageId: update.Message.Id, $"Текущая Дата/Время: *{DateTime.Now: dd:MM:yyyy HH:mm:ss}*", ParseMode.Markdown);
        }
    }
    public class OnProfileShow : ICommand
    {
        public string CommandName => "/profile";
        private readonly TelegramBotClient bot_client;
        /// <summary>
        /// Команда вложенного меню просмотра текущего профиля пользователя из <see cref="user_profiles"/>
        /// </summary>
        /// <param name="client"></param>
        public OnProfileShow(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message.Chat.Id;

            if (!UserProfile.user_profiles.TryGetValue(chatID, out UserProfile? myProfile))
            {
                myProfile = new UserProfile();
                UserProfile.user_profiles[chatID] = myProfile;
            }

            string profileInfo = $"*Профиль*\n" +
                $"*Имя* {myProfile.Name}\n" +
                $"*Любимая тема общения* {myProfile.FavouriteTopic}\n" +
                $"*Стиль общения* {myProfile.ChatStyle}\n\n";

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(3500);
            await bot_client.EditMessageText(chatID, messageId: update.Message.Id, profileInfo, ParseMode.Markdown);
        }
    }
    public class OnProfileChange : ICommand
    {
        public string CommandName => "profile_change";
        private readonly TelegramBotClient bot_client;
        private readonly Dictionary<long, UserProfile> user_profiles = [];
        /// <summary>
        /// Команда-кнопка вызова вложенного меню для изменения профиля пользователя <see cref="user_profiles"/>
        /// </summary>
        /// <param name="client"></param>
        public OnProfileChange(TelegramBotClient client) => bot_client = client;
        public async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message.Chat.Id;
            var userId = callback.From.Id;

            if (!user_profiles.ContainsKey(userId))
                    user_profiles[userId] = new UserProfile();

            string profileSettings = $"Для изменения профиля Бота, используй: `/set_name Имя`, `/set_topic Тема`, `/set_style Стиль`.";
            await Task.Delay(2000);
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, profileSettings, ParseMode.Markdown, replyMarkup: GetProfileKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
}
