using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ratatouille
{
    internal class AiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> GenerateCodeAsync(
            string prompt,
            string apiKey,
            string model = "gemini-2.5-flash-lite",
            double temperature = 0.1,
            string systemPrompt = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return "// Eroare: Nicio cheie API configurata! Mergi la Tools -> Options -> Ratatouille AI.";

            if (string.IsNullOrWhiteSpace(systemPrompt))
                systemPrompt =
                    "You are a coding assistant integrated directly into Visual Studio. " +
                    "Reply ONLY with the exact code requested, no explanations, no Markdown fences.";

            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

            try
            {
                string fullPrompt = systemPrompt + "\n\nUser request: " + prompt;

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = fullPrompt } } }
                    },
                    generationConfig = new
                    {
                        temperature = temperature
                    }
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();
                JObject parsedJson = JObject.Parse(responseJson);

                string generatedCode = parsedJson["candidates"][0]["content"]["parts"][0]["text"].ToString();

                generatedCode = generatedCode
                    .Replace("```csharp", "")
                    .Replace("```javascript", "")
                    .Replace("```python", "")
                    .Replace("```typescript", "")
                    .Replace("```", "")
                    .Trim();

                return generatedCode;
            }
            catch (Exception ex)
            {
                return $"// Eroare la conectarea cu Gemini ({model}): {ex.Message}";
            }
        }
    }
}