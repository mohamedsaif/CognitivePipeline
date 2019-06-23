﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Models
{
    /// <summary>
    /// Encapsulate all the details about a specific cognitive processing step
    /// </summary>
    public class CognitiveStep
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("cognitiveServiceType")]
        public CognitiveService ServiceType { get; set; }

        [JsonProperty("rawOutput")]
        public string RawOutput { get; set; }

        [JsonProperty("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; }

        [JsonProperty("isSuccessful")]
        public bool IsSuccessful { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
