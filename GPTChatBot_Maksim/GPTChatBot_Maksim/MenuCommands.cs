using Utilities;
using static Keyboards.KeyboardGetter;

namespace Commands
{
    public class OnInlineMenu(TelegramBotClient client) : TextCommand(client)
    {
        public override string CommandName => "/inline_menu";

        public override async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message!.Chat.Id;
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(300);
            await bot_client.SendMessage(chatID, "Вот мое главное меню, выбирай что хочешь:", ParseMode.Markdown, replyMarkup: GetInlineKeyboard());
        }
    }

    public class OnAssistantMenuShow(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "assistant_mode";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;

            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "Режим ассистента:", replyMarkup: GetAssistantKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }

    public class OnWebUrlOpen(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "websites";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "Сайты:", replyMarkup: GetWebsiteMenuKeyboard());
        }
    }

    public class OnChatSettings(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "chat_settings";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "Меню чата:", replyMarkup: GetChatMenuKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }

    public class OnRPSMenuShow(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "rps_minigame";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            await Task.Delay(500);
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, "*Ваш ход:*", ParseMode.Markdown, replyMarkup: GetRockPaperScissorsMenu());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }

    public class OnProfileChange(TelegramBotClient client) : CallbackCommand(client)
    {
        public override string CommandName => "profile_change";

        public override async Task ExecuteAsync(CallbackQuery callback)
        {
            var chatID = callback.Message!.Chat.Id;
            var userId = callback.From.Id;

            if (!UserProfile.user_profiles.ContainsKey(userId))
                UserProfile.user_profiles[userId] = new UserProfile();

            string profileSettings = $"Для изменения профиля Бота, используй: `/set_name Имя`, `/set_topic Тема`, `/set_style Стиль`.";
            await Task.Delay(2000);
            await bot_client.EditMessageText(chatID, messageId: callback.Message.MessageId, profileSettings, ParseMode.Markdown, replyMarkup: GetProfileKeyboard());
            await bot_client.AnswerCallbackQuery(callback.Id);
        }
    }
}
