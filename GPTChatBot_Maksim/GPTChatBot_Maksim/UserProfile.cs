namespace GPTChatBot_Maksim.Utilities
{
    internal class UserProfile
    {
        public static readonly Dictionary<long, UserProfile> user_profiles = [];
        internal string Name { get; set; } = "Тестовое Имя";
        internal string FavouriteTopic { get; set; } = "Общее";
        internal string ChatStyle { get; set; } = "Нейтральный";
        // [] - эвивалент, тоже что и string[] = new {};
        internal static readonly string[] AvailableStyles =
        [
            "нейтральный", "саркастичный", "вежливый", "раздражающий"
        ];
    }
}
