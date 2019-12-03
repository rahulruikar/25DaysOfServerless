using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Day3.Models;
using System.Collections.Generic;
using Octokit;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Day3
{
    public static class GitHubWebhook
    {
        [FunctionName("GitHubWebHook")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var gitPushEvent = JsonConvert.DeserializeObject<GitPushEvent>(requestBody);
            var _gitHubClient = new GitHubClient(new ProductHeaderValue("my-test-app"));

            foreach (var item in gitPushEvent.Commits)
            {
                GitHubCommit commit = await _gitHubClient.Repository.Commit.Get(gitPushEvent.Repository.Id, item.Id);
                foreach (var file in commit.Files)
                {
                    if (file.Filename.ToLower().EndsWith(".png"))
                    {
                        ImageEntity entity = new ImageEntity(item.Author.Username, Guid.NewGuid().ToString())
                        {
                            ImageUrl = file.RawUrl
                        };

                        try
                        {
                            CloudStorageAccount storageAccount;
                            storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageConnection"));
                            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                            CloudTable table = tableClient.GetTableReference(Environment.GetEnvironmentVariable("TableName"));
                            await table.CreateIfNotExistsAsync();
                            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
                            TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                            ImageEntity imageEntity = result.Result as ImageEntity;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception Occurred : {ex.Message}");
                        }
                    }
                }
            }

            return (ActionResult)new OkObjectResult("Ok");
        }
    }
}
