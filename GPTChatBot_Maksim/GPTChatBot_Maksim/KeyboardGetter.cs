namespace GPTChatBot_Maksim.Utilities
{
    /// <summary>
    /// Клавиатуры Телеграмм-Бота
    /// </summary>
    internal static class KeyboardGetter
    {
        /// <summary>
        /// Основная клавиатура, содержащая вложенные разные меню
        /// </summary>
        /// <returns></returns>
        internal static InlineKeyboardMarkup GetInlineKeyboard() => new(
        [
            [InlineKeyboardButton.WithCallbackData("Меню сайтов", "websites")],
            [InlineKeyboardButton.WithCallbackData("Быстрый набор реплик", "quick_answers")],
            [InlineKeyboardButton.WithCallbackData("Режим ассистента", "assistant_mode")],
            [InlineKeyboardButton.WithCallbackData("Камень, Ножницы, Бумага...", "rps_minigame")],
            [InlineKeyboardButton.WithCallbackData("Настройки чата", "chat_settings")],
            [InlineKeyboardButton.WithCallbackData("Изменить профиль", "profile_change")]
        ]);
        /// <summary>
        /// Меню быстрых наборов реплик
        /// </summary>
        /// <returns></returns>
        internal static ReplyKeyboardMarkup GetReplyKeyboard() => new(
        [
            [new("Привет, Бот!"), new("Как дела?")],
            [new("Выдать случайное число"), new("Какой сегодня день?")],
            [new("Случайный факт!"), new("Выдать шутку")],
            [new("Подсказать Дату и Время")],
            [new("Прощай, Бот!")]
        ])
        { ResizeKeyboard = true, OneTimeKeyboard = false };
        /// <summary>
        /// Вложенное меню для настройки "Режима Ассистента"
        /// </summary>
        /// <returns></returns>
        internal static InlineKeyboardMarkup GetAssistantKeyboard() => new(
        [
            [InlineKeyboardButton.WithCallbackData("Включить AI-Ассистента", "assistant_on")],
            [InlineKeyboardButton.WithCallbackData("Выключить AI-Ассистента", "assistant_off")],
            [InlineKeyboardButton.WithCallbackData("<- Назад", "back_to_main")]
        ]);
        /// <summary>
        /// Вложенное меню для ссылок на разные источники
        /// </summary>
        /// <returns></returns>
        internal static InlineKeyboardMarkup GetWebsiteMenuKeyboard() => new(
        [
            [InlineKeyboardButton.WithUrl("Откуда я беру факты:", "https://ru.wikipedia.org")],
            [InlineKeyboardButton.WithUrl("Мой друг-помощник:", "https://chatgpt.com/")],
            [InlineKeyboardButton.WithUrl("Актуальные забавные шутки", "https://www.geeksforgeeks.org/short-jokes/")],
            [InlineKeyboardButton.WithCallbackData("<- Назад", "back_to_main")]
        ]);
        /// <summary>
        /// Вложенное меню настроек чатов
        /// </summary>
        /// <returns></returns>
        internal static InlineKeyboardMarkup GetChatMenuKeyboard() => new(
        [
            [InlineKeyboardButton.WithCallbackData("Новый чат", "new_chat")],
            [InlineKeyboardButton.WithCallbackData("Выбрать чат", "select_chat")],
            [InlineKeyboardButton.WithCallbackData("Загрузить чат", "load_chat")],
            [InlineKeyboardButton.WithCallbackData("Удалить чат", "delete_chat")],
            [InlineKeyboardButton.WithCallbackData("<- Назад", "back_to_main")]
        ]);
        /// <summary>
        /// Вложенное меню обработки выбора мини-игры "Камень, Ножницы, Бумага..."
        /// </summary>
        /// <returns></returns>
        internal static InlineKeyboardMarkup GetRockPaperScissorsMenu() => new(
        [
            [InlineKeyboardButton.WithCallbackData("Камень", "rps_Камень")],
            [InlineKeyboardButton.WithCallbackData("Ножницы", "rps_Ножницы")],
            [InlineKeyboardButton.WithCallbackData("Бумага", "rps_Бумага")],
            [InlineKeyboardButton.WithCallbackData("Колодец", "rps_Колодец")],
        ]);
        /// <summary>
        /// Вложенное меню настроек профиля пользователя
        /// </summary>
        /// <returns></returns>
        internal static InlineKeyboardMarkup GetProfileKeyboard() => new(
        [
            [InlineKeyboardButton.WithCallbackData("Изменить имя", "profile_set_name")],
            [InlineKeyboardButton.WithCallbackData("Изменить тему", "profile_set_topic")],
            [InlineKeyboardButton.WithCallbackData("Изменить стиль общения", "profile_set_style")],
            [InlineKeyboardButton.WithCallbackData("<- Назад", "back_to_main")]
        ]);
    }
}
