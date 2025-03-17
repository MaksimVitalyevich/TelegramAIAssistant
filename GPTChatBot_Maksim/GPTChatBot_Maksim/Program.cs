namespace TelegrammBot
{
    internal class Program
    {
        static async Task Main()
        {
            SetLoggingPath();

            var bot = new AI_TeleBot(new ArgumentNullException("Нету токена для бота 'GPTMaksim'!"));
            // обработчик логов в файл расширения .log, записывается в корневую папку проекта
            await bot.StartAsync();
            while (true)
            {
                Console.WriteLine("Введите команду для остановки бота...\n");
                var command = Console.ReadLine();
                if (command == "stop" || command == "shutdown" || command == "exit" || command == "sleep")
                {
                    Console.WriteLine("[DEBUG] AI-Бот завершил свою работу...");
                    Logger.INSTANCE.LogMessage(LogLevel.Info, "Бот завершил работу...");
                    break;
                }
                    
                else
                    Console.WriteLine($"Команда - {command} не распознана!");
            }
        }
        static void SetLoggingPath()
        {
            Logger.INSTANCE.SetConfiguration(@"..\..\..\MaksimBotExceptions", LogLevel.Info);
        }
    }
}
