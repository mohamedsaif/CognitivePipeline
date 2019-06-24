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
        //Remember always that storage containers allows only lower case letters
        public const string StorageContainerName = "cognitivefiles";
        public const string DbName = "CognitiveFilesDb";
        public const string DbCognitiveFilesContainer = "CognitiveFiles";
        public const string DbUserAccountsContainer = "UserAccounts";
        public const string NewQueueName = "newreq";
        public const string CallbackQueueName = "callback";
    }
}
