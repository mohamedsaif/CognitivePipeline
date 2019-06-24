using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Abstractions
{
    public interface IQueueRepository
    {
        Task<bool> QueueMessage(string message);
        Task<CloudQueueMessage> GetMessage();
        Task<bool> DeleteMessage(CloudQueueMessage message);
        Task<int> GetQueueLength();
    }
}
