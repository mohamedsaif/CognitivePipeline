using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions
{
    /// <summary>
    /// Hold a central list of all static configurations that can remain the same across all deployments
    /// </summary>
    public static class AppConstants
    {
        public const string StorageContainerName = "CognitiveFiles";
        public const string DBName = "CognitiveFilesDb";
        public const string DBContainerName = "CognitiveFiles";
    }
}
