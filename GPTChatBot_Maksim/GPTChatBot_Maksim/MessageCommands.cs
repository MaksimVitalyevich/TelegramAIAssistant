using TelegrammBot;
using static Dictionaries.ReplyDictionaries;
using static Keyboards.KeyboardGetter;

namespace Commands
{
    public class OnQuickReply(TelegramBotClient client) : TextCommand(client)
    {
        private readonly Random random = new();

        public override string CommandName => "/quick_answers";

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

        public override async Task ExecuteAsync(Update update)
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
            AI_TeleBot.chatHistoryManager.SaveMessage(chatID, update.Message.Text);
        }
    }

    public class OnMessageReply(TelegramBotClient client) : TextCommand(client)
    {
        private readonly Random random = new();

        public override string CommandName => "message_reply"; // Напрямую не используется, но нужна для структуры 

        public override async Task ExecuteAsync(Update update)
        {
            if (update.Message?.Text is not { } userMsg) return;
            var chatID = update.Message.Chat.Id;

            if (!AI_TeleBot.isAssistant_Mode.ContainsKey(chatID) || !AI_TeleBot.isAssistant_Mode[chatID])
            {
                await bot_client.SendChatAction(chatID, ChatAction.Typing);
                await Task.Delay(2000);
                await bot_client.SendMessage(chatID, Misunderstanding[random.Next(Misunderstanding.Count)]);
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

            string aiResponse = await AI_TeleBot.AI_ASSISTANT.GenerateResponse(userMsg, chatID);
            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1600);
            await bot_client.SendMessage(chatID, aiResponse);
            AI_TeleBot.chatHistoryManager.SaveMessage(chatID, userMsg);
        }
    }
}
