namespace Utilities
{
    public class UserProfile
    {
        private static readonly string ProfileDirectory = @"..\..\..\..\UserProfiles";
        // [] - эвивалент, тоже что и string[] = new {};
        internal static readonly string[] AvailableStyles =
        [
            "нейтральный", "саркастичный", "вежливый", "раздражающий"
        ];
        public static readonly Dictionary<long, UserProfile> user_profiles = [];

        internal string Name { get; set; } = "Тестовое Имя";
        internal string Language { get; set; } = "rus";
        internal string UsingAIModel { get; set; } = "Qwen/Qwen2.5-7B-Instruct";
        internal string FavouriteTopic { get; set; } = "Общее";
        internal string ChatStyle { get; set; } = "Нейтральный";
        internal int WarningCount { get; set; } = 0;
        internal bool IsBanned { get; set; } = false;
        internal DateTime? LastWarningDate { get; set; }

        public static void SaveProfilesToFile()
        {
            if (!Directory.Exists(ProfileDirectory))
                Directory.CreateDirectory(ProfileDirectory);

            string json = JsonSerializer.Serialize(user_profiles, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(ProfileDirectory, "profiles.json"), json);
        }

        public static void LoadProfilesFromFile()
        {
            string filePath = Path.Combine(ProfileDirectory, "profiles.json");
            if (!File.Exists(filePath)) return;

            try
            {
                string json = File.ReadAllText(filePath);
                var loadedProfiles = JsonSerializer.Deserialize<Dictionary<long, UserProfile>>(json);

                if (loadedProfiles != null)
                    foreach (var kvp in loadedProfiles)
                        user_profiles[kvp.Key] = kvp.Value;
            }
            catch
            {
                Logger.INSTANCE.LogMessage(LogLevel.Error, "Ошибка при загрузке профилей.");
            }
        }
    }
}
