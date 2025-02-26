namespace GPTChatBot_Maksim
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new AI_TeleBot("..\\..\\..\\MaksimBotExceptions", new ArgumentNullException("Нету токена для бота 'GPTMaksim'!"));
            await bot.StartAsync();
            while (true)
            {
                Console.WriteLine("Введите команду для остановки бота...");
                var command = Console.ReadLine();
                if (command == "stop" || command == "shutdown" || command == "exit" || command == "sleep")
                    break;
                else
                    Console.WriteLine($"Команда - {command} не распознана!");
            }
        }
    }
}
