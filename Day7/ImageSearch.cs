using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Web.Http;

namespace Day7
{    public class ImageSearch
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ImageSearch(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
        }

        [FunctionName("ImageSearch")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                string searchCriteria = req.Query["image"];
                var fileContent = await SearchAndDownloadImage(searchCriteria);
                return new FileContentResult(fileContent, "application/octet-stream");
            }
            catch(Exception ex)
            {
                return (ActionResult)new ExceptionResult(ex, false);
            }
        }

        public async Task<byte[]> SearchAndDownloadImage(string searchCriteria)
        {
            string url = $"{_configuration.GetValue<string>("UNSPLASH_API_ENDPOINT")}/search/photos?query={searchCriteria}&page=1&per_page=1";
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Client-ID {_configuration.GetValue<string>("UNSPLASH_ACCESS_KEY")}");
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            if ((int)response.StatusCode == StatusCodes.Status200OK)
            {
                var content = JsonConvert.DeserializeObject<UnSplashApiResponse>(await response.Content.ReadAsStringAsync());
                return await _httpClient.GetByteArrayAsync(content.Results[0].Links.Download);
            }
            throw new FileNotFoundException();
        }
    }
}
