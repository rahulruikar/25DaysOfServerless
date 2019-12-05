using Day5.Interfaces;
using Day5.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Day5
{
    public class Letter
    {
        [JsonProperty(PropertyName = "who")]
        public string Who { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }

    public class SentimentAnalyzer
    {
        private readonly ITextTranslationService _textTranslationService;
        private readonly ISentimentAnalysisService _sentimentAnalysiService;

        public SentimentAnalyzer(ITextTranslationService textTranslationService,
            ISentimentAnalysisService sentimentAnalysiService)
        {
            _textTranslationService = textTranslationService;
            _sentimentAnalysiService = sentimentAnalysiService;
        }
        
        [FunctionName("NaughtyOrNice")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Letter letter = JsonConvert.DeserializeObject<Letter>(requestBody);

            TextTranslationResult textTranslationResult = await _textTranslationService.TranslateText(letter.Message, "en");
            var sentimentanalysis = _sentimentAnalysiService.PerformSentimentAnalysis(textTranslationResult);
            // Convert final result into original language
            var correspondenceResult = $"{letter.Who}, You are being {sentimentanalysis}";
            // Reply the result in originally detected language.
            var translatedCorrespondenceResult = await _textTranslationService.TranslateText(correspondenceResult, textTranslationResult.detectedLanguage);
            return new OkObjectResult($"{translatedCorrespondenceResult.text}");
        }
    }
}
