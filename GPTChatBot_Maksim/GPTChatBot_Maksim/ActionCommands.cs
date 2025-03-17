using TelegrammBot;
using static Keyboards.KeyboardGetter;

namespace Commands
{
    public class OnBackToMain(TelegramBotClient client) : CallbackCommand(client)
    {
        private readonly string gpt_bot_welcome = "*Чем могу помочь? Выбери действие ниже:*";

        public override string CommandName => "back_to_main";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, gpt_bot_welcome, ParseMode.Markdown, replyMarkup: GetInlineKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }

    public class OnAssistantOn(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "assistant_on";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;

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

    public class OnAssistantOff(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "assistant_off";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;

            // Проверка, что режим AI уже выключен
            if (!AI_TeleBot.isAssistant_Mode.TryGetValue(chatID, out bool IsDisabled) && !IsDisabled)
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

    public class OnNewChat(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "new_chat";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            string newChatID = AI_TeleBot.chatHistoryManager.NewChatHistory(chatID);

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1500);
            await bot_client.SendMessage(chatID, $"Создан новый чат: *{newChatID}*.\nНовый чат - активирован!", ParseMode.Markdown);
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }

    public class OnSelectChat(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "select_chat";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            var chatList = AI_TeleBot.chatHistoryManager.GetChatList(chatID);

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

    public class OnSwitchChat(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "switch_chat";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            string chatName = callback.Data.Replace("switch_chat|", "");

            if (AI_TeleBot.chatHistoryManager.SetActiveChat(chatID, chatName))
                await bot_client.SendMessage(chatID, $"Чат переключен на: *{chatName}*", ParseMode.Markdown);
            else
                await bot_client.SendMessage(chatID, "Ошибка: этот чат не найден.");

            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }

    public class OnLoadChat(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "load_chat";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            string chatHistory = AI_TeleBot.chatHistoryManager.LoadChatHistory(chatID);
            await Task.Delay(1200);
            await bot_client.SendMessage(chatID, $"История чата:*\n\n{chatHistory}*", ParseMode.Markdown);
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }

    public class OnDeleteChat(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "delete_chat";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            string chatName = callback.Data.Replace("delete_chat_", "");

            await Task.Delay(1200);

            if (AI_TeleBot.chatHistoryManager.DeleteChatFromHistory(chatID, chatName))
                await bot_client.SendMessage(chatID, $"Чат *{chatName}* удален!", ParseMode.Markdown);
            else
                await bot_client.SendMessage(chatID, "Такой чат не найден.");

            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }

    public class OnRPSGameStart(TelegramBotClient client) : CallbackCommand(client)
    {
        private static readonly string[] rps_choices = ["Камень", "Ножницы", "Бумага", "Колодец"];
        private readonly Random random = new();

        public override string CommandName => "rps_"; // Не фиксированное имя, будет проверяться через CommandManager

        public override async Task ExecuteAsync(CallbackQuery callback)
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

            var chatID = callback.Message!.Chat.Id;

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1500);
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, resultMsg, ParseMode.Markdown);
            await Task.Delay(3000);
            await bot_client.SendChatAction(chatID, ChatAction.Typing);

            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "Вот мое главное меню, выбирай что хочешь:", replyMarkup: GetInlineKeyboard());
        }
    }

    public class OnProfileNameSet(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "set_name";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            var userId = callback.From.Id;

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(700);
            await bot_client.SendMessage(chatID, $"Имя изменено (автоматически) на: {AI_TeleBot.userProfile.Name}");
        }
    }

    public class OnProfileTopicSet(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "set_topic";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            var userId = callback.From.Id;

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(700);
            await bot_client.SendMessage(chatID, $"Тема изменена (автоматически) на: {AI_TeleBot.userProfile.FavouriteTopic}");
        }
    }

    public class OnProfileStyleSet(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "set_topic";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            var userId = callback.From.Id;

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(700);
            await bot_client.SendMessage(chatID, $"Стиль общения изменено (автоматически) на: {AI_TeleBot.userProfile.FavouriteTopic}");
        }
    }
}
