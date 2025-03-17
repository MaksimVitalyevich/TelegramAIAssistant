namespace Utilities
{
    /// <summary>
    /// Менеджер управления чатов
    /// </summary>
    public class ChatHistoryManager
    {
        // [] - упрощенная запись, эквивалент new Dictionary<T, T>();
        private static readonly Dictionary<long, Dictionary<string, List<string>>> chat_histories = [];
        private static readonly Dictionary<long, string> active_chats = [];

        /// <summary>
        /// Получение списка чатов
        /// </summary>
        /// <param name="chatID">ID-номер чата.</param>
        /// <returns>Список полученных чатов.</returns>
        public List<string> GetChatList(long chatID)
        {
            return chat_histories.ContainsKey(chatID) ? chat_histories[chatID].Keys.ToList() : [];
        }
        /// <summary>
        /// Активация как созданных чатов, так и выбранных
        /// </summary>
        /// <param name="chatID">ID-номер чата.</param>
        /// <param name="chatName">Имя чата.</param>
        /// <returns></returns>
        public bool SetActiveChat(long chatID, string chatName)
        {
            if (chat_histories.ContainsKey(chatID) && chat_histories[chatID].ContainsKey(chatName))
            {
                active_chats[chatID] = chatName;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Создание нового чата
        /// </summary>
        /// <param name="chatID">ID-номер чата.</param>
        /// <returns>Созданная история для чатов.</returns>
        public string NewChatHistory(long chatID)
        {
            if (!chat_histories.ContainsKey(chatID))
                chat_histories[chatID] = new Dictionary<string, List<string>>();

            string newchatID = $"Chat_{chat_histories[chatID].Count + 1}";

            chat_histories[chatID][newchatID] = [];
            active_chats[chatID] = newchatID;

            return newchatID;
        }
        /// <summary>
        /// Автосохранение сообщении в чате
        /// </summary>
        /// <param name="chatID">ID-номер чата.</param>
        /// <param name="message">Сообщение.</param>
        public void SaveMessage(long chatID, string message)
        {
            if (!active_chats.ContainsKey(chatID)) return;

            string chatName = active_chats[chatID];

            if (!chat_histories.ContainsKey(chatID))
                chat_histories[chatID] = new Dictionary<string, List<string>>();

            if (!chat_histories.ContainsKey(chatID))
                chat_histories[chatID][chatName] = [];

            chat_histories[chatID][chatName].Add(message);
            Logger.INSTANCE.LogMessage(LogLevel.Info, $"[ChatID: {chatID}][{chatName}] {message}");
        }
        /// <summary>
        /// Загрузка выбранного чата
        /// </summary>
        /// <param name="chatID">ID-номер чата.</param>
        /// <returns>Загруженный предыдущий выбранный чат.</returns>
        public string LoadChatHistory(long chatID)
        {
            if (!active_chats.ContainsKey(chatID)) return "Активных чатов - нет!";

            string chatName = active_chats[chatID];

            return chat_histories.TryGetValue(chatID, out var userChats) && userChats.TryGetValue(chatName, out var messages) 
                ? string.Join("\n", messages) : "История чата пуста.";
        }
        /// <summary>
        /// Удаление выбранного чата
        /// </summary>
        /// <param name="chatID">ID-номер чата.</param>
        /// <param name="chatName">Имя чата.</param>
        /// <returns></returns>
        public bool DeleteChatFromHistory(long chatID, string chatName)
        {
            if (chat_histories.ContainsKey(chatID) && chat_histories[chatID].ContainsKey(chatName))
            {
                chat_histories[chatID].Remove(chatName);
                return true;
            }
            return false;
        }
    }
}
