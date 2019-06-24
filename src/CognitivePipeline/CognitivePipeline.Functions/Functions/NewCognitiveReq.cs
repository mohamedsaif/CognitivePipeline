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
using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.Models.DTO;
using CognitivePipeline.Functions.Validation;
using Newtonsoft.Json.Converters;

namespace CognitivePipeline.Functions.Functions
{
    /// <summary>
    /// Azure Function that primarily parse & save both the Cognitive File details to Cosmos DB, Cognitive File bytes to Azure Storage container
    /// </summary>
    public class NewCognitiveReq
    {
        private readonly ICognitiveFilesRepository cognitiveFilesRepo;
        private readonly IUserAccountRepository userAccountsRepo;
        private readonly IStorageRepository filesStorageRepo;

        /// <summary>
        /// Leveraging the new Azure Functions Dependency Injection by sending common services in the constructor
        /// </summary>
        /// <param name="filesRepo">Cosmos Db repository for Cognitive Files</param>
        public NewCognitiveReq(ICognitiveFilesRepository filesRepo, IUserAccountRepository usersRepo, IStorageRepository storageRepo)
        {
            cognitiveFilesRepo = filesRepo;
            userAccountsRepo = usersRepo;
            filesStorageRepo = storageRepo;
        }

        /// <summary>
        /// Initiate new cognitive pipeline processing for a document
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("NewCognitiveReq")]
        public async Task<IActionResult> Run(
            //Trigger
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,

            //Output
            [Queue("newreq", Connection = "cognitivePipelineStorageConnection")]ICollector<string> newReqsQueue,

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
                var cognitiveFileDTO = JsonConvert.DeserializeObject<CognitiveFileDTO>(fileInfoJson);

                var cognitiveFile = ValidateCognitiveFile.ValidateForSubmission(cognitiveFileDTO, userAccountsRepo);

                //Get file bytes
                var fileData = provider.Contents[1];
                var fileBytes = await fileData.ReadAsByteArrayAsync();

                var url = await filesStorageRepo.CreateFileAsync(cognitiveFile.FileName, fileBytes);

                cognitiveFile.FileUrl = url;

                await cognitiveFilesRepo.AddAsync(cognitiveFile);

                newReqsQueue.Add(JsonConvert.SerializeObject(cognitiveFile));

                return new CreatedResult(cognitiveFile.FileName, null);
            }
            catch (Exception ex)
            {
                log.LogError($"####### FAILED: {ex.Message}");
                return new BadRequestObjectResult($"Request submission failed");
            }
        }
    }
}
