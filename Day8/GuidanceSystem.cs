using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Day8
{
    public class GuidanceSystem
    {
        public class StatusDetails
        {
            [JsonProperty(PropertyName ="id")]
            public string Id { get; set; }

            [JsonProperty(PropertyName ="value")]
            public string Value { get; set; }
        }

        [FunctionName("SendStatusUpdate")]
        public Task Run([CosmosDBTrigger(
            databaseName: "reindeerguidancesystem",
            collectionName: "StatusData",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true,
            ConnectionStringSetting = "AzureCosmosDBConnectionString"
            )]IReadOnlyList<Document> input,
            [SignalR(HubName = "guidancesystem")]IAsyncCollector<SignalRMessage> signalRMessages, ILogger log)
        {

            var updates = JsonConvert.DeserializeObject<StatusDetails>(input[0].ToString());
            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "SystemUpdates",
                    Arguments = new[] { updates.Value }
                });
        }

    }
}
