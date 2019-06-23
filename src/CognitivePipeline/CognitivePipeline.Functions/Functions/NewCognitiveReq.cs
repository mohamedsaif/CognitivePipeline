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

namespace CognitivePipeline.Functions.Functions
{
    /// <summary>
    /// Azure Function that primarily parse & save both the Cognitive File details to Cosmos DB, Cognitive File bytes to Azure Storage container
    /// </summary>
    public static class NewCognitiveReq
    {
        [FunctionName("NewCognitiveReq")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("******* NewCognitiveReq starting");

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await req.Content.ReadAsMultipartAsync(provider);

                //Get training file attributes (name and regions)
                var trainingFileInfo = provider.Contents[0];
                var trainingFileJson = await trainingFileInfo.ReadAsStringAsync();
                var trainingFileDTO = JsonConvert.DeserializeObject<DTOTrainingFile>(trainingFileJson);

                //Get training file bytes
                var trainingFileData = provider.Contents[1];
                var trainingFileBytes = await trainingFileData.ReadAsByteArrayAsync();

                var workspaceManager = await ODWorkspaceManagerHelper.SetWorkspaceManager(trainingFileDTO.OwnerId, false);

                var workspace = await workspaceManager.GetWorkspaceAsync(trainingFileDTO.OwnerId, true);

                var newFileName = workspaceManager.AddTrainingFile(trainingFileDTO.FileName, trainingFileBytes, Mapper.Map<List<ObjectRegion>>(trainingFileDTO.Regions));

                await workspaceManager.ValidateAndSaveWorkspace();

                return new CreatedResult(newFileName, null);
            }
            catch (Exception ex)
            {
                log.LogError($"FAILED: {ex.Message}");
                return new BadRequestObjectResult($"Addition of the file failed");
            }
        }
    }
}
