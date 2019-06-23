using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.Data;
using CognitivePipeline.Functions.Utils;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

[assembly: FunctionsStartup(typeof(CognitivePipeline.Functions.Startup))]

namespace CognitivePipeline.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var endpoint = GlobalSettings.GetKeyValue("cosmosDbEndpoint");
            var key = GlobalSettings.GetKeyValue("cosmosDbKey");
            var client = new DocumentClient(new Uri(endpoint), key, new ConnectionPolicy { EnableEndpointDiscovery = false });
            builder.Services.AddSingleton<ICosmosDbClientFactory>((s) =>
            {
                return new CosmosDbClientFactory(AppConstants.DbName, new List<string> { AppConstants.DbCognitiveFilesContainer, AppConstants.DbUserAccountsContainer }, client);
            });

            var storageConnection = GlobalSettings.GetKeyValue("cognitivePipelineStorageConnection");
            builder.Services.AddSingleton<IStorageRepository>((s) =>
            {
                return new AzureBlobStorageRepository(storageConnection, AppConstants.StorageContainerName);
            });

            //Register our cosmos db repository for Cognitive Files :)
            builder.Services.AddSingleton<ICognitiveFilesRepository, CognitiveFileRepository>();
            builder.Services.AddSingleton<IUserAccountRepository, UserAccountRepository>();
        }
    }
}