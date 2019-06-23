using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Utils
{
    public class GlobalSettings
    {
        //https://integration.team/2017/09/25/retrieve-azure-key-vault-secrets-using-azure-functions-managed-service-identity/
        //Declaring static fields will allow reusability without creating them everytime
        static HttpClient client = new HttpClient();
        static AzureServiceTokenProvider azureServiceTokenProvider;
        static KeyVaultClient kvClient;

        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        public static async Task<string> GetKeyVaultSecret(string secretName)
        {
            //Don't forget to provision a service identity on your function. Refer to documentation for further information
            azureServiceTokenProvider = new AzureServiceTokenProvider();

            kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback), client);

            string secretValue = (await kvClient.GetSecretAsync(Environment.GetEnvironmentVariable(secretName)))?.Value;

            return secretValue;
        }

        public static async Task<string> GetKeyValue(string keyName, bool isSecret)
        {
            if (isSecret)
                return await GetKeyVaultSecret(keyName);

            return GetEnvironmentVariable(keyName);
        }

        public static string GetKeyValue(string keyName)
        {
            bool isSecret = GetEnvironmentVariable("Env") == "Protected";
            if (isSecret)
            {
                //return await GetKeyVaultSecret(keyName);
                Task<string> t = GetKeyVaultSecret(keyName);
                t.Wait();
                return t.Result;
            }
            return GetEnvironmentVariable(keyName);
        }
    }
}
