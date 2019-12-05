using Day5.Interfaces;
using Day5.Models;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Day5.Services
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly string _subscriptionKey;
        private readonly string _textAnalysisEndpoint;

        public SentimentAnalysisService(IConfiguration configuration)
        {
            _subscriptionKey = configuration.GetValue<string>("AnalyticsSubscriptionKey");
            _textAnalysisEndpoint = configuration.GetValue<string>("AnalyticsEndpoint");
        }

        public string PerformSentimentAnalysis(TextTranslationResult input)
        {
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(_subscriptionKey);
            TextAnalyticsClient client = new TextAnalyticsClient(credentials)
            {
                Endpoint = _textAnalysisEndpoint
            };

            var result = client.Sentiment(input.text, "en");
            if (result.Score >= 0.5)
            {
                return "Nice";
            }
            else
            {
                return "Naughty";
            }
        }
    }

    class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        private readonly string apiKey;

        public ApiKeyServiceClientCredentials(string apiKey)
        {
            this.apiKey = apiKey;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            request.Headers.Add("Ocp-Apim-Subscription-Key", this.apiKey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }
    }
}
