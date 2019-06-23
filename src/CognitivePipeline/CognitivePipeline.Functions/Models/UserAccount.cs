using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Models
{
    public class UserAccount : BaseModel
    {
        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("accountType")]
        public string AccountType { get; set; }

        [JsonProperty("authenticationOptions")]
        public string AuthenticationOptions { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        [JsonProperty("displayName")]
        public virtual string DisplayName { get; set; }

        [JsonProperty("facePersonId")]
        public string FacePersonId { get; set; }
    }
}
