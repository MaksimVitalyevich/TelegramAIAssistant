using TelegrammBot;
using Utilities;
using static Keyboards.KeyboardGetter;

namespace Commands
{
    public class OnStart(TelegramBotClient client) : TextCommand(client)
    {
        private string GPTBotWelcome { get; set; } = "Приветствую! Я *GPTMaksim*\nПростой бот для разговора с AI возможностями!\n" +
            "Умею:\n 🔹 Отвечать на ваши вопросы;\n 🔹 Обрабатывать файлы;\n 🔹 Предлагать полезные факты или смешно пошутить;\n" +
            " 🔹 Работать с чатами;\n 🔹 Работать с профилем пользователя;\n 🔹 Ну и просто провести с тобой хорошее время!";

        public override string CommandName => "/start";

        public override async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message!.Chat.Id;

            await bot_client.SendMessage(chatID, GPTBotWelcome, ParseMode.Markdown);
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(5500);
            await bot_client.SendMessage(chatID, "Вот мое главное меню, выбирай что хочешь:", replyMarkup: GetInlineKeyboard());
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1500);
            await bot_client.SendMessage(chatID, "Нужна помощь с командами? Воспользуйтесь /help!");
        }
    }

    public class OnHelp(TelegramBotClient client) : TextCommand(client)
    {
        public override string CommandName => "/help";

        public override async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message!.Chat.Id;

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

    public class OnShowAiModel(TelegramBotClient client) : TextCommand(client)
    {
        public override string CommandName => "/ai_model";

        public override async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message!.Chat.Id;
            string response = $"Используется модель ИИ общения: {AI_TeleBot.userProfile.UsingAIModel}; \nЛокализация: {AI_TeleBot.userProfile.Language}";

            await bot_client.SendMessage(chatID, response);
        }
    }

    public class OnProfileShow(TelegramBotClient client) : TextCommand(client)
    {
        public override string CommandName => "/profile";

        public override async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message!.Chat.Id;

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
            await bot_client.SendMessage(chatID, profileInfo, ParseMode.Markdown);
        }
    }

    public class OnDateTimePrint(TelegramBotClient client) : TextCommand(client)
    {
        public override string CommandName => "/datetime";

        public override async Task ExecuteAsync(Update update)
        {
            var chatID = update.Message!.Chat.Id;
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(2500);
            await bot_client.SendMessage(chatID, $"Текущая Дата/Время: *{DateTime.Now: dd:MM:yyyy HH:mm:ss}*", ParseMode.Markdown);
        }
    }
}
