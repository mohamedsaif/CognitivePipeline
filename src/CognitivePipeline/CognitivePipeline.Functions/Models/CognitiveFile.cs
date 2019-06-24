using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Models
{
    public class CognitiveFile : BaseModel
    {
        [JsonProperty("ownerId")]
        public string OwnerId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("mediaType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FileMediaType MediaType { get; set; }

        [JsonProperty("cognitivePipelineActions")]
        public List<CognitiveStep> CognitivePipelineActions { get; set; } = new List<CognitiveStep>();

        [JsonProperty("isProcessed")]
        public bool IsProcessed { get; set; }

        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
