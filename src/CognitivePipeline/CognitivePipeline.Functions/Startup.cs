using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

[assembly: FunctionsStartup(typeof(CognitivePipeline.Functions.Startup))]

namespace CognitivePipeline.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();
            //builder.Services.AddSingleton((s) =>
            //{
            //    return new CosmosClient(Environment.GetEnvironmentVariable("COSMOSDB_CONNECTIONSTRING"));
            //});
            //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
        }
    }
}