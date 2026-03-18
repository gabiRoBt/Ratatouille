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

        // We now pass the API key directly as a parameter to avoid lifecycle issues
        public async Task<string> GenerateCodeAsync(string prompt, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "// Error: No API key configured! Please go to Tools -> Options -> Ratatouille AI to add your key.";
            }

            string apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={apiKey}";
            try
            {
                string fullPrompt = "You are a coding assistant integrated directly into Visual Studio. " +
                                    "You must reply ONLY with the exact code requested. " +
                                    "CRITICAL: Do not include any explanations, greetings, or Markdown formatting (like ```csharp). " +
                                    "I need raw text that can be directly compiled.\n\n" +
                                    "User request: " + prompt;

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = fullPrompt } } }
                    },
                    generationConfig = new
                    {
                        temperature = 0.1
                    }
                };

                string jsonContent = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                string responseJson = await response.Content.ReadAsStringAsync();
                JObject parsedJson = JObject.Parse(responseJson);

                string generatedCode = parsedJson["candidates"][0]["content"]["parts"][0]["text"].ToString();

                generatedCode = generatedCode.Replace("```csharp", "").Replace("```", "").Trim();

                return generatedCode;
            }
            catch (Exception ex)
            {
                return $"// Error connecting to Gemini: {ex.Message}";
            }
        }
    }
}