using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CognitivePipeline.Functions.Models;
using System.Net;
using System.Net.Http;

namespace CognitivePipeline.Functions.Functions
{
    public static class GetAvailableServices
    {
        /// <summary>
        /// Return a list of the current available services in the cognitive pipeline
        /// </summary>
        /// <param name="req">Hold the request object</param>
        /// <param name="log">Injecting the logger</param>
        /// <returns></returns>
        [FunctionName("GetAvailableServices")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("******* GetAvailableServices starting");

            var result = new CognitiveServicesListResult {
                StatusCode = HttpStatusCode.OK
            };

            return new OkObjectResult(result);
        }
    }
}
