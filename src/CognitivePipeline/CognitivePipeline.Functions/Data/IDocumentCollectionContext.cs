using CognitivePipeline.Functions.Models;
using Microsoft.Azure.Documents;

namespace CognitivePipeline.Functions.Data
{
    internal interface IDocumentCollectionContext<T> where T : BaseModel
    {
        string CollectionName { get; }

        string GenerateId(T entity);

        PartitionKey ResolvePartitionKey(string entityId);
    }
}