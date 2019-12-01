using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Day1
{
    public class DreidelSpinner
    {
        enum DreidelOptions
        {
            Nun = 1,
            Gimmel = 2,
            Hay = 3,
            Shin = 4
        };

        [FunctionName("spin-dreidel")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processing dreidel spin request");

            var randomizer = new Random();
            DreidelOptions value = (DreidelOptions)randomizer.Next(Enum.GetNames(typeof(DreidelOptions)).Length);
            return (ActionResult)new OkObjectResult(Enum.GetName(typeof(DreidelOptions), value));
        }
    }
}
