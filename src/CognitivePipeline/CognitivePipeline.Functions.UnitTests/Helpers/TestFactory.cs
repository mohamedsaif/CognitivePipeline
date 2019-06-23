using CognitivePipeline.Functions.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace CognitivePipeline.Functions.UnitTests.Helpers
{
    public class TestFactory
    {
        public static List<CognitiveFileDTO> GetData()
        {
            CognitiveFileDTO newFile1 = new CognitiveFileDTO
            {
                FileName = "mosaif_id.png",
                MediaType = Models.FileMediaType.Image,
                Origin = "UnitTests",
                OwnerId = "783ed915-8e5d-418b-ade0-8cc5f217ecf7"
            };

            newFile1.CognitivePipelineActions = new List<CognitiveStepDTO>
            {
                new CognitiveStepDTO { ServiceType = Models.CognitiveService.OCR },
                new CognitiveStepDTO { ServiceType = Models.CognitiveService.FaceDetection }
            };

            CognitiveFileDTO newFile2 = new CognitiveFileDTO
            {
                FileName = "mohamed-saif.jpg",
                MediaType = Models.FileMediaType.Image,
                Origin = "UnitTests",
                OwnerId = "783ed915-8e5d-418b-ade0-8cc5f217ecf7"
            };

            newFile2.CognitivePipelineActions = new List<CognitiveStepDTO>
            {
                new CognitiveStepDTO { ServiceType = Models.CognitiveService.ImageAnalysis },
                new CognitiveStepDTO { ServiceType = Models.CognitiveService.OCR }
            };

            return new List<CognitiveFileDTO> { newFile1, newFile2 };
        }

        public static List<Byte[]> GetFiles()
        {
            var result = new List<byte[]>
            {
                TestFilesHelper.GetTestFile("mosaif_id.png"),
                TestFilesHelper.GetTestFile("mohamed-saif.jpg")
            };
            return result;
        }

        public static HttpRequestMessage CreateMultipartRequest(string url, Dictionary<string, HttpContent> content)
        {
            //var request = new DefaultHttpRequest(new DefaultHttpContext())
            //{
            //    Method = "POST",
            //};
            
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url);

            //using (var postContent = new MultipartFormDataContent("----MyBoundary"))
            //{
            //    foreach (KeyValuePair<string, HttpContent> entry in content)
            //    {
            //        postContent.Add(entry.Value, entry.Key);
            //    }

            //    req.Content = postContent;
            //}
            var postContent = new MultipartFormDataContent("----MyBoundary");

            foreach (KeyValuePair<string, HttpContent> entry in content)
            {
                postContent.Add(entry.Value, entry.Key);
            }

            req.Content = postContent;

            return req;
        }

        public static HttpRequestMessage GetMultipartRequest(int index)
        {
            var fileInfo = GetData()[index];
            var stringContent = new StringContent(JsonConvert.SerializeObject(fileInfo));

            var fileData = GetFiles()[index];
            var streamContent = new StreamContent(new MemoryStream(fileData));

            var payload = new Dictionary<string, HttpContent>();
            payload.Add("info", stringContent);
            payload.Add("data", streamContent);

            var req = CreateMultipartRequest("", payload);

            return req;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;

            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}