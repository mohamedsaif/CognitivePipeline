using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Data
{
    public interface ICosmosDbClientFactory
    {
        ICosmosDbClient GetClient(string collectionName);
        Task EnsureDbSetupAsync();
    }
}