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
using CognitivePipeline.Functions.Models;

namespace CognitivePipeline.Functions.Functions
{
    /// <summary>
    /// Azure Function that primarily parse & save both the Cognitive File details to Cosmos DB, Cognitive File bytes to Azure Storage container
    /// </summary>
    public static class NewCognitiveReq
    {
        /// <summary>
        /// Initiate new cognitive pipeline processing for a document
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("NewCognitiveReq")]
        public static async Task<IActionResult> Run(
            //Trigger
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,

            //Logger
            ILogger log)
        {
            log.LogInformation("******* NewCognitiveReq starting");

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await req.Content.ReadAsMultipartAsync(provider);

                //Get cognitive file attributes
                var fileInfo = provider.Contents[0];
                var fileInfoJson = await fileInfo.ReadAsStringAsync();
                var cognitiveFile = JsonConvert.DeserializeObject<CognitiveFile>(fileInfoJson);

                //Get file bytes
                var fileData = provider.Contents[1];
                var fileBytes = await fileData.ReadAsByteArrayAsync();

                return new CreatedResult("REPLACEWITHID", null);
            }
            catch (Exception ex)
            {
                log.LogError($"####### FAILED: {ex.Message}");
                return new BadRequestObjectResult($"Request submission failed");
            }
        }
    }
}
