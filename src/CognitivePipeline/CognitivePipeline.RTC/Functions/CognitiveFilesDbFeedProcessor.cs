using System.Collections.Generic;
using System.Threading.Tasks;
using CognitivePipeline.RTC.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CognitivePipeline.RTC.Functions
{
    public static class CognitiveFilesDbFeedProcessor
    {
        [FunctionName("CognitiveFilesDbFeedProcessor")]
        public static async Task Run(
            //Trigger
            [CosmosDBTrigger(
            databaseName: "CognitiveFilesDb",
            collectionName: "CognitiveFiles",
            ConnectionStringSetting = "CogntiveFilesDbConnection",
            LeaseCollectionName = "leases")]IReadOnlyList<CognitiveFile> input,

            //Output
            [SignalR(HubName = AppConstants.SignalRHub)]IAsyncCollector<SignalRMessage> signalRMessages,

            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);

                foreach(var item in input)
                {
                    await signalRMessages.AddAsync(new SignalRMessage
                    {
                        Target = "ProcessingUpdate",
                        Arguments = new[] { item },
                        UserId = item.OwnerId
                    });
                }
            }
        }
    }
}
