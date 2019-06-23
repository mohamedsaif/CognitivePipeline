using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Models.DTO
{
    public class CognitiveStepDTO
    {
        [JsonProperty("cognitiveServiceType")]
        public CognitiveServiceType ServiceType { get; set; }
    }
}
