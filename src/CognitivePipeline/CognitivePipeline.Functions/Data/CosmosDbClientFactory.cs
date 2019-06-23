using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Data
{
    public class CosmosDbClientFactory : ICosmosDbClientFactory
    {
        private readonly string _databaseName;
        private readonly List<string> _collectionNames;
        private readonly IDocumentClient _documentClient;

        public CosmosDbClientFactory(string databaseName, List<string> collectionNames, IDocumentClient documentClient)
        {
            _databaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
            _collectionNames = collectionNames ?? throw new ArgumentNullException(nameof(collectionNames));
            _documentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
        }

        public ICosmosDbClient GetClient(string collectionName)
        {
            if (!_collectionNames.Contains(collectionName))
            {
                throw new ArgumentException($"Unable to find collection: {collectionName}");
            }

            return new CosmosDbClient(_databaseName, collectionName, _documentClient);
        }

        public async Task EnsureDbSetupAsync()
        {
            await CreateDatabaseIfNotExists();
            await CreateColltionsIfNotExists();
        }

        private async Task CreateDatabaseIfNotExists()
        {
            try
            {
                await _documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseName));
            }
            catch (DocumentClientException e)
            {
                //Database do not exists! Create it
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _documentClient.CreateDatabaseAsync(new Database { Id = _databaseName });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateColltionsIfNotExists()
        {
            foreach (var collectionName in _collectionNames)
            {
                try
                {
                    await _documentClient.ReadDocumentCollectionAsync(
                        UriFactory.CreateDocumentCollectionUri(_databaseName, collectionName));
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        var docCollection = new DocumentCollection { Id = collectionName };
                        string partionKey = "";

                        if (collectionName == AppConstants.DbCognitiveFilesContainer)
                            partionKey = "/ownerId";
                        else if (collectionName == AppConstants.DbUserAccountsContainer)
                            partionKey = "/accountId";
                        else
                            partionKey = "/id";

                        if (string.IsNullOrEmpty(partionKey))
                            docCollection.PartitionKey.Paths.Add($"/{partionKey}");

                        await _documentClient.CreateDocumentCollectionAsync(
                            UriFactory.CreateDatabaseUri(_databaseName),
                            docCollection,
                            new RequestOptions { OfferThroughput = 400 });
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
