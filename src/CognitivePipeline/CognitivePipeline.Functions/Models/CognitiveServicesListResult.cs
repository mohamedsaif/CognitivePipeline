using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Models
{
    public class CognitiveServicesListResult : BaseResult
    {
        [JsonProperty("availableServices")]
        public string[] AvaiableServices {
            get
            {
                return Enum.GetNames(typeof(CognitiveServiceType));
            }
        }
    }
}
