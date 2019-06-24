using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.Models;
using CognitivePipeline.Functions.Utils;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Orchestrator
{
    public class CognitivePipelineOrchestrator
    {
        private readonly ICognitiveFilesRepository cognitiveFilesRepo;
        private readonly IUserAccountRepository userAccountsRepo;
        private readonly IStorageRepository filesStorageRepo;

        public CognitivePipelineOrchestrator(ICognitiveFilesRepository filesRepo, IUserAccountRepository usersRepo, IStorageRepository storageRepo)
        {
            cognitiveFilesRepo = filesRepo;
            userAccountsRepo = usersRepo;
            filesStorageRepo = storageRepo;
        }

        [FunctionName("CognitivePipelineOrchestrator_QueueStart")]
        public async Task QueueStart(
            //Trigger
            [QueueTrigger("newreq", Connection = "cognitivePipelineStorageConnection")]CognitiveFile cognitiveFile,

            //Orchestrator Client
            [OrchestrationClient]DurableOrchestrationClient starter,

            ILogger log)
        {
            try
            {
                // Function input comes from the new queue message.
                //Adding contrable instance id. This would be used easily to retrieve the orchestrator context
                var instanceId = $"{cognitiveFile.Id}-cp";
                string returnedId = await starter.StartNewAsync("CognitivePipelineOrchestrator", instanceId, cognitiveFile);
                log.LogInformation($"******* Started orchestration with ID = '{instanceId}'.");
            }
            catch (Exception e)
            {
                log.LogError($"####### Failed to start via queue trigger: {e.Message}");
            }
        }

        [FunctionName("CognitivePipelineOrchestrator")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var input = context.GetInput<CognitiveFile>();

            var parallelTasks = new List<Task<CognitiveStep>>();

            //Fanout all cognitive tasks in parallel
            foreach (var step in input.CognitivePipelineActions)
            {
                step.FileUrl = input.FileName;
                parallelTasks.Add(context.CallActivityAsync<CognitiveStep>($"CognitivePipeline_{step.ServiceType}", step));
            }

            await Task.WhenAll(parallelTasks);

            foreach (var task in parallelTasks)
            {
                var updatedStep = task.Result;
                var originalStepIndex = input.CognitivePipelineActions.FindIndex(s => s.ServiceType == updatedStep.ServiceType);
                input.CognitivePipelineActions.RemoveAt(originalStepIndex);
                input.CognitivePipelineActions.Insert(originalStepIndex, updatedStep);
            }

            await context.CallActivityAsync("CognitivePipeline_Callback", input);
        }

        [FunctionName("CognitivePipeline_OCR")]
        public async Task<CognitiveStep> CognitivePipeline_OCR([ActivityTrigger] CognitiveStep input, ILogger log)
        {
            log.LogInformation($"******* Starting OCR");

            string key = GlobalSettings.GetKeyValue("computerVisionKey");
            string endpoint = GlobalSettings.GetKeyValue("computerVisionEndpoint");

            ComputerVisionClient computerVision = new ComputerVisionClient(
                new Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials(key),
                new System.Net.Http.DelegatingHandler[] { })
            { Endpoint = endpoint };

            var data = await filesStorageRepo.GetFileAsync(input.FileUrl);

            var ocrResult = await computerVision.RecognizePrintedTextInStreamAsync(true, new MemoryStream(data));

            input.IsSuccessful = true;
            input.Confidence = -1;
            input.LastUpdatedAt = DateTime.UtcNow;
            input.RawOutput = JsonConvert.SerializeObject(ocrResult);

            return input;
        }

        [FunctionName("CognitivePipeline_FaceDetectionBasic")]
        public async Task<CognitiveStep> CognitivePipeline_FaceDetectionBasic([ActivityTrigger] CognitiveStep input, ILogger log)
        {
            log.LogInformation($"******* Starting Face Detection");

            string key = GlobalSettings.GetKeyValue("computerVisionKey");
            string endpoint = GlobalSettings.GetKeyValue("computerVisionEndpoint");

            ComputerVisionClient computerVision = new ComputerVisionClient(
                new Microsoft.Azure.CognitiveServices.Vision.ComputerVision.ApiKeyServiceClientCredentials(key),
                new System.Net.Http.DelegatingHandler[] { })
            { Endpoint = endpoint };

            var data = await filesStorageRepo.GetFileAsync(input.FileUrl);

            var detectionResult = await computerVision.AnalyzeImageInStreamAsync(new MemoryStream(data), new List<VisualFeatureTypes> { VisualFeatureTypes.Faces });

            input.IsSuccessful = true;
            input.Confidence = detectionResult.Faces.Count > 0 ? 1 : 0;
            input.LastUpdatedAt = DateTime.UtcNow;
            input.RawOutput = JsonConvert.SerializeObject(detectionResult);

            return input;
        }

        [FunctionName("CognitivePipeline_FaceDetection")]
        public async Task<CognitiveStep> CognitivePipeline_FaceDetection([ActivityTrigger] CognitiveStep input, ILogger log)
        {
            log.LogInformation($"******* Starting Face Detection");

            string key = GlobalSettings.GetKeyValue("faceKey");
            string endpoint = GlobalSettings.GetKeyValue("faceEndpoint");

            IFaceClient faceClient = new FaceClient(
                        new Microsoft.Azure.CognitiveServices.Vision.Face.ApiKeyServiceClientCredentials(key),
                        new System.Net.Http.DelegatingHandler[] { })
                        { Endpoint = endpoint };

            var data = await filesStorageRepo.GetFileAsync(input.FileUrl);

            IList<FaceAttributeType> faceAttributes = new FaceAttributeType[]
                                           {
                                                FaceAttributeType.Gender, FaceAttributeType.Age,
                                                FaceAttributeType.Smile, FaceAttributeType.Emotion,
                                                FaceAttributeType.Glasses, FaceAttributeType.Hair
                                           };
            try
            {
                using (Stream imageFileStream = new MemoryStream(data))
                {
                    // The second argument specifies to return the faceId, while
                    // the third argument specifies not to return face landmarks.
                    IList<DetectedFace> faceList =
                        await faceClient.Face.DetectWithStreamAsync(
                            imageFileStream, true, false, faceAttributes);
                    input.IsSuccessful = true;
                    input.Confidence = faceList.Count > 0 ? 1 : 0;
                    input.LastUpdatedAt = DateTime.UtcNow;
                    input.RawOutput = JsonConvert.SerializeObject(faceList);
                    return input;
                }
            }
            // Catch and display Face API errors.
            catch (APIErrorException e)
            {
                log.LogError($"####### Failed to detect faces: {e.Message}");
                input.IsSuccessful = false;
                input.Confidence = 0;
                input.LastUpdatedAt = DateTime.UtcNow;
                input.Status = e.Message;
                return input;
            }
        }

        [FunctionName("CognitivePipeline_Callback")]
        public async Task<CognitiveFile> CognitivePipeline_Callback([ActivityTrigger] CognitiveFile file, ILogger log)
        {
            log.LogInformation($"******* Starting Callback");

            file.IsProcessed = true;

            await cognitiveFilesRepo.UpdateAsync(file);

            return file;
        }
    }
}