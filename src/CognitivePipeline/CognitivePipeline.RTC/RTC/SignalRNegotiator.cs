using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace CognitivePipeline.RTC.RTC
{
    public static class SignalRNegotiator
    {
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            //Trigger
            [HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,

            //Connection Binding
            [SignalRConnectionInfo
            (HubName = AppConstants.SignalRHubName, 
            ConnectionStringSetting = AppConstants.SignalRConnection)]
            SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}