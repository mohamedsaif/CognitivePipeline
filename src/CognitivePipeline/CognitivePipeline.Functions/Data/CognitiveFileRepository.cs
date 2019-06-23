using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.Models;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Data
{
    public class CognitiveFileRepository : CosmosDbRepository<CognitiveFile>, ICognitiveFilesRepository
    {
        public CognitiveFileRepository(ICosmosDbClientFactory factory) : base(factory) { }

        public override string CollectionName { get; } = "todoItems";
        public override string GenerateId(CognitiveFile entity) => $"{entity.OwnerId}:{Guid.NewGuid()}";
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':')[0]);
    }

}
}
