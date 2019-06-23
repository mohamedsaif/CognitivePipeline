using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Models.DTO
{
    public class CognitiveFileDTO
    {
        [JsonProperty("ownerId")]
        public string OwnerId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("mediaType")]
        public FileMediaType MediaType { get; set; }

        [JsonProperty("cognitivePipelineActions")]
        public List<CognitiveStep> CognitivePipelineActions { get; set; } = new List<CognitiveStep>();

        [JsonProperty("origin")]
        public string Origin { get; set; }
    }
}
