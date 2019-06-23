using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Models
{
    /// <summary>
    /// Base class for all result types
    /// </summary>
    public class BaseResult
    {
        /// <summary>
        /// Usually GUID to uniquely identify the result
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Custom defined request id to be associated with the result
        /// </summary>
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        /// <summary>
        /// Returns the HTTP status code (like 200 or HTTP OK)
        /// </summary>
        [JsonProperty("statusCode")]
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Custom defined error codes
        /// </summary>
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// Provide context about the result
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Returns false if the result includes a problem
        /// </summary>
        public bool HasError
        {
            get { return (!string.IsNullOrEmpty(ErrorCode)) || (StatusCode != 0 && StatusCode != HttpStatusCode.OK); }
        }
    }
}
