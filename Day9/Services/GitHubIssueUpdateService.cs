using Day9.Interfaces;
using Microsoft.Extensions.Configuration;
using Octokit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Day9.Services
{
    public class GitHubIssueUpdateService : IGitHubIssueUpdate
    {
        private readonly IConfiguration _configuration;

        public GitHubIssueUpdateService(IConfiguration configuration)
        {
            _configuration = configuration;
          }
        public async Task AddCommentToIssue(long repositoryId, int issueNumber, string issueCreator)
        {
            try
            {
                var gitHubClient = new GitHubClient(new ProductHeaderValue("my-day9-challenge-app"));
                gitHubClient.Credentials = new Credentials(_configuration.GetValue<string>("GITHUB_PERSONAL_ACCESS_TOKEN"));
                string commentOnIssue = $"Thank you @{issueCreator} for creating this issue!\nHave a Happy Holiday season!";
                await gitHubClient.Issue.Comment.Create(repositoryId, issueNumber, commentOnIssue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to add comment to github issue : {ex.Message}");
                throw;
            }
            return;
        }
    }
}
