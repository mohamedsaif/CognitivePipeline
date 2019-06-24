using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Models.DTO
{
    public class CognitiveStepDTO
    {
        [JsonProperty("serviceType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public CognitiveServiceType ServiceType { get; set; }
    }
}
