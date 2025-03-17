namespace Utilities
{
    internal class CensorDetector
    {
        private static readonly HashSet<string> FORBIDDEN_WORDS = 
        [
            "хуй", "пизда", "блять", "нахуй", "мудак", "уебок", "пиздабол", "ублюдок", "хуесос"
        ];

        private bool ContainsForbiddenWords(string message, UserProfile profile)
        {
            string lowerMsg = message.ToLower();

            foreach (var word in FORBIDDEN_WORDS)
            {
                if (lowerMsg.Contains(word))
                {
                    profile.WarningCount++;
                    profile.LastWarningDate = DateTime.Now;

                    if (profile.WarningCount >= 5)
                        profile.IsBanned = true;

                    return true;
                }
            }

            return false;
        }

        private void UnbanUser(UserProfile profile)
        {
            if (profile.IsBanned)
            {
                profile.IsBanned = false;
                profile.WarningCount = 0;
                Logger.INSTANCE.LogMessage(LogLevel.Info, $"Пользователь {profile.Name} - был разбанен.");
            }
        }

        public string CheckMessageForCensorship(long chatId, string userMsg)
        {
            if (!UserProfile.user_profiles.TryGetValue(chatId, out var profile))
            {
                profile = new UserProfile { Name = $"User_{chatId}" };
                UserProfile.user_profiles[chatId] = profile;
            }

            if (profile.IsBanned)
                return "❌ Вы были заблокированы за неадекватное поведение.";

            if (ContainsForbiddenWords(userMsg, profile))
            {
                int warningsLeft = 5 - profile.WarningCount;

                if (warningsLeft > 0)
                    return $"⚠️ Внимание! Ваще сообщение содержит запрещенное слово/слова. " +
                        $"Осталось {warningsLeft} предупреждении до полной блокировки.";
                else
                    return "❌ Вы были заблокированы за нарушение правил общения.";
            }

            if (profile.WarningCount > 0)
            {
                profile.WarningCount = 0;
                return "✅ Спасибо, что больше не используете нецензурную лексику в ваших сообщениях. Объясните пожалуйста, " +
                    "почему вы вели себя так грубо?";
            }

            return string.Empty; // нет нарушении
        }
    }
}
