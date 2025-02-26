namespace GPTChatBot_Maksim
{
    internal class UserProfile
    {
        internal string Name { get; set; } = "Тестовое Имя";
        internal string FavouriteTopic { get; set; } = "Общее";
        internal string ChatStyle { get; set; } = "Нейтральный";
        internal static readonly string[] AvailableStyles =
        [
            "нейтральный", "саркастичный", "вежливый", "раздражающий"
        ];
    }
}
