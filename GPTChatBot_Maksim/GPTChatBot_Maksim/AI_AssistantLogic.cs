using System.Text;
using System.Text.Json;

namespace GPTChatBot_Maksim
{
    internal class AI_AssistantLogic(string openAiApiKey)
    {
        private static readonly HttpClient httpClient = new();
        private readonly string apiKey = openAiApiKey;

        internal async Task<string> GenerateResponse(string userMsg)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo", // Используется GPT-3.5
                messages = new[]
                {
                    new {role = "system", content = "Ты разговорный AI-бот, который дружелюбно общается с пользователями."},
                    new {role = "user", content = userMsg}
                },
                max_tokens = 200
            };

            var requestJson = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Headers = { { "Authorization", $"Bearer {apiKey}" } },
                Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            var answer = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            return answer ?? "Я не понял твоего вопроса.";
        }
    }
}
