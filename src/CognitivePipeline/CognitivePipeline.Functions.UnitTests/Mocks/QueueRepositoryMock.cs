using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.UnitTests.Helpers;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.UnitTests.Mocks
{
    public class QueueRepositoryMock : IQueueRepository
    {
        public Task<bool> DeleteMessage(CloudQueueMessage message)
        {
            return Task.FromResult<bool>(true);
        }

        public Task<CloudQueueMessage> GetMessage()
        {
            return Task.FromResult<CloudQueueMessage>(new CloudQueueMessage(JsonConvert.SerializeObject(TestFactory.CognitiveFileDTOs[0])));
        }

        public Task<int> GetQueueLength()
        {
            return Task.FromResult<int>(1);
        }

        public Task<bool> QueueMessage(string message)
        {
            return Task.FromResult<bool>(true);
        }
    }
}
