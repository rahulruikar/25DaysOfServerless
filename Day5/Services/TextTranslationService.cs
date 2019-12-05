using Day5.Interfaces;
using Day5.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Day5.Services
{

    public class TextTranslationService : ITextTranslationService
    {
        private readonly string _subscriptionKey;
        private readonly string _translationServiceEndpoint;
        private readonly string _route;

        public TextTranslationService(IConfiguration configuration)
        {
            _subscriptionKey = configuration.GetValue<string>("TextTranslatorSubscriptionKey");
            _translationServiceEndpoint = configuration.GetValue<string>("TextTranslatorEndpoint");
            _route = "/translate?api-version=3.0";
        }

        public async Task<TextTranslationResult> TranslateText(string inputText, string language)
        {
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                Console.WriteLine(_translationServiceEndpoint);
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(_translationServiceEndpoint + _route + "&to=" + language);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                string result = await response.Content.ReadAsStringAsync();

                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                return new TextTranslationResult
                {
                    detectedLanguage = deserializedOutput[0].DetectedLanguage.Language,
                    text = deserializedOutput[0].Translations[0].Text
                };
            }
        }
    }
}
