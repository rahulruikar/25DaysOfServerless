using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using Day9.Models;
using Day9.Interfaces;
using System.Net;

namespace Day9
{
    public class GitWebhook
    {
        private readonly IGitHubIssueUpdate _gitHubIssueUpdateService;

        public GitWebhook(IGitHubIssueUpdate gitHubIssueUpdateService)
        {
            _gitHubIssueUpdateService = gitHubIssueUpdateService;
        }

        [FunctionName("ThankYou")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing github webhook.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GitHubIssueEvent githubEvent = JsonConvert.DeserializeObject<GitHubIssueEvent>(requestBody);

            if (githubEvent.action == "opened")
            {
                try
                {
                    await _gitHubIssueUpdateService.AddCommentToIssue(githubEvent.Repository.Id, githubEvent.Issue.Number, githubEvent.Issue.User.Login);
                }
                catch(Exception)
                {
                    return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
                }
                
            }

            return (ActionResult)new OkObjectResult("");

        }
    }
}
