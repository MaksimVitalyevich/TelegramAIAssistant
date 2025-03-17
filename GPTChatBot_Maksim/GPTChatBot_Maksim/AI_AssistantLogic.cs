using Utilities;

namespace Assistant
{
    /// <summary>
    /// Обработка AI-ассистента для генерации ответов-сообщении (основа DeepInfra)
    /// </summary>
    public class AI_AssistantLogic(string apiKey)
    {
        private const string API_URL = "https://api.deepinfra.com/v1/openai/chat/completions";
        private static readonly HttpClient HTTPCLIENT = new();
        
        private readonly string apiKey = apiKey;
        private readonly CensorDetector censorer = new();

        /// <summary>
        /// Генерация AI-ответов
        /// </summary>
        /// <param name="userMsg">Сообщение пользователя.</param>
        /// <param name="chatId">ИД чата.</param>
        /// <returns>Сгенерированный осмысленный ответ ассистента.</returns>
        internal async Task<string> GenerateResponse(string userMsg, long chatId)
        {
            string censorResult = censorer.CheckMessageForCensorship(chatId, userMsg);

            if (!string.IsNullOrEmpty(censorResult))
                return censorResult;

            var requestBody = new
            {
                model = "Qwen/Qwen2.5-7B-Instruct",
                messages = new[]
                {
                    new {role = "system", content = "Ты простой разговорный дружелюбный собеседник, с которым можно поболтать на любые темы."},
                    new {role = "user", content = userMsg}
                },
                max_tokens = 512
            };

            HTTPCLIENT.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var requestJson = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, API_URL)
            {
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            var response = await HTTPCLIENT.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка API: {response.StatusCode} - {errorMessage}");
                return "❌ Ошибка запроса к AI! Возможно, сервер временно недоступен.";
            }

            response.EnsureSuccessStatusCode();
            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var answer = doc.RootElement.GetProperty("choices")[0]
                                        .GetProperty("message")
                                        .GetProperty("content")
                                        .GetString();

            return answer ?? "Я не понял твоего вопроса.";
        }

        /// <summary>
        /// Обработка текстовых файлов
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="bot_client">Клиент.</param>
        /// <returns>Выдает результат итогового анализа после прочтения файла.</returns>
        internal async Task ProcessTextFileAsync(Message message, TelegramBotClient bot_client)
        {
            if (message.Document == null)
            {
                await bot_client.SendChatAction(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1000);
                await bot_client.SendMessage(message.Chat.Id, "Файл не найден!");
                return;
            }

            // проверка расширении файла
            var allowedExtensions = new List<string> { ".txt", ".rtf", ".md", ".csv" };
            string fileName = message.Document.FileName ?? "Unnamed";
            string fileExtension = Path.GetExtension(fileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                await bot_client.SendChatAction(message.Chat.Id, ChatAction.Typing);
                await Task.Delay(1500);
                await bot_client.SendMessage(message.Chat.Id, $"Поддерживаются только текстовые файлы {allowedExtensions.Count}");
                return;
            }

            await bot_client.SendChatAction(message.Chat.Id, ChatAction.UploadDocument);
            var file = await bot_client.GetFile(message.Document.FileId);

            using var stream = new MemoryStream();
            await Task.Delay(2200);
            await bot_client.DownloadFile(file.FilePath, stream);
            stream.Position = 0;

            using var reader = new StreamReader(stream);
            string fileContent = await reader.ReadToEndAsync();

            await SendToGPT(fileContent, message.Chat.Id, bot_client);
        }

        /// <summary>
        /// Отправка документов-файлов на http сервер для обработки
        /// </summary>
        /// <param name="text">Отправленный текст.</param>
        /// <param name="chatID">ИД чата.</param>
        /// <param name="bot_client">Клиент.</param>
        /// <returns>Полученный ответ от используемой модели ИИ.</returns>
        internal static async Task SendToGPT(string text, long chatID, TelegramBotClient bot_client)
        {
            string apiUrl = "https://free.churchless.tech/v1/chat/completions"; ;

            var requestData = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = text }
                }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
            var response = await HTTPCLIENT.PostAsync(apiUrl, jsonContent);
            var responseString = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(responseString);
            string reply = document.RootElement.
                GetProperty("choices")[0].
                GetProperty("message").
                GetProperty("content").
                GetString() ?? "Ошибка обработки.";

            await bot_client.SendChatAction(chatID, ChatAction.Typing);
            await Task.Delay(1800);
            await bot_client.SendMessage(chatID, reply);
        }
    }
}
