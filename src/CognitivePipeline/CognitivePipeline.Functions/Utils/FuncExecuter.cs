using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Utils
{
    /// <summary>
    /// Utility class to help with getting both environment and Azure Key Vault settings
    /// </summary>
    public class FuncExecuter
    {
        public static HttpClient httpClient = new HttpClient();
        private static string functionKey = GlobalSettings.GetKeyValue("FunctionGlobalKey");

        /// <summary>
        /// Execute a HTTP triggered function and return the execution result
        /// </summary>
        /// <param name="functionUri">FQDN of the target function</param>
        /// <param name="content">Json body to be sent to target function</param>
        /// <returns></returns>
        public static async Task<IActionResult> ExecFunction(string functionUri, StringContent content)
        {
            var result = "";
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("x-functions-key", functionKey);

            using (HttpResponseMessage response = (await httpClient.PostAsync(functionUri, content)))
            using (HttpContent responseContent = response.Content)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = await responseContent.ReadAsStringAsync();
                }
                else
                {
                    //TODO: Handle the non 200 code response messages here

                    return (ActionResult)new BadRequestObjectResult(result);
                }
            }

            return (ActionResult)new OkObjectResult(result);
        }
    }
}
