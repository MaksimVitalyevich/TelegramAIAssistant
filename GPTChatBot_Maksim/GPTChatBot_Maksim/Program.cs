namespace GPTChatBot_Maksim
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var bot = new AI_TeleBot("..\\..\\..\\MaksimBotExceptions", new ArgumentNullException("Нету токена для бота 'GPTMaksim'!"));
            // обработчик логов в файл расширения .log, записывается в корневую папку проекта
            var logger = new Logger("..\\..\\..\\MaksimBotExceptions", LogLevel.Info);
            await bot.StartAsync();
            while (true)
            {
                Console.WriteLine("Введите команду для остановки бота...\n");
                var command = Console.ReadLine();
                if (command == "stop" || command == "shutdown" || command == "exit" || command == "sleep")
                {
                    Console.WriteLine("[DEBUG] AI-Бот завершил свою работу...");
                    logger.LogMessage(LogLevel.Info, "Бот завершил работу...");
                    break;
                }
                    
                else
                    Console.WriteLine($"Команда - {command} не распознана!");
            }
        }
    }
}
