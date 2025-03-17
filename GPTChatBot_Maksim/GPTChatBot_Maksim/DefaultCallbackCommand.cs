using Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegrammBot
{
    public interface IDefaultCallbackCommand
    {
        Task ExecuteFallbackAsync(CallbackQuery callback);
    }
    public class DefaultCallbackCommand(TelegramBotClient client) : CallbackCommand(client), IDefaultCallbackCommand
    {
        public override string CommandName => "default_callback";

        public override Task ExecuteAsync(CallbackQuery callback)
        {
            return Task.CompletedTask;
        }

        public async Task ExecuteFallbackAsync(CallbackQuery callback)
        {
            var chatID = callback.Message?.Chat.Id;
            if (chatID == null) return;

            await bot_client.AnswerCallbackQuery(callback.Id, "Слишком старый или неизвестный запрос.");
            await bot_client.SendMessage(chatID, "Команда не распознана или истек срок действия ответа.");
        }
    }
}
