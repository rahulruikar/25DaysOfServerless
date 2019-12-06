using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Chronic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Day6
{
    public class SlackChatIntegration
    {
        public class UserInput
        {
            public string messgeText { get; set; }
            public TimeSpan timervalue { get; set; }
        };

        public enum TaskScheduleStatus
        {
            Created,
            Activated
        };

        public class TaskDetail
        {
            public string message { get; set; }
            public TaskScheduleStatus taskStatus { get; set; }
        };
 
        [FunctionName("SlackChatIntegration")]
        public  async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var userinput = context.GetInput<UserInput>();
            await context.CallActivityAsync<TaskDetail>("NotifyUser", new TaskDetail { message = userinput.messgeText, taskStatus = TaskScheduleStatus.Created });
            await context.CreateTimer(context.CurrentUtcDateTime.Add(userinput.timervalue), CancellationToken.None);
            await context.CallActivityAsync<TaskDetail>("NotifyUser", new TaskDetail { message = userinput.messgeText, taskStatus = TaskScheduleStatus.Activated });

            return;
        }

        [FunctionName("NotifyUser")]
        public  async Task SayHello([ActivityTrigger] TaskDetail taskDetail, ILogger log)
        {
            string slackWebHook = "https://hooks.slack.com/services/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";
            var completeMessage = taskDetail.taskStatus == TaskScheduleStatus.Created ? $"{taskDetail.message} has been scheduled" : $"Your Scheduled {taskDetail.message} to happen now";
            var json = JsonConvert.SerializeObject(new
            {
                text = completeMessage
            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            var response = await client.PostAsync(slackWebHook, data);
            return;
        }

        [FunctionName("SlackChatIntegration_HttpStart")]
        public  async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            string message = await req.Content.ReadAsStringAsync();
            var parser = new Parser();
            var span = parser.Parse(message);
 
            var userinput = new UserInput
            {
                messgeText = message,
                timervalue = span.ToTime() - DateTime.Now
            };
            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("SlackChatIntegration", userinput);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}