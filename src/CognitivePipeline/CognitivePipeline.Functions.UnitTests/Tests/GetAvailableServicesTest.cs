using CognitivePipeline.Functions.Functions;
using CognitivePipeline.Functions.Models;
using CognitivePipeline.Functions.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CognitivePipeline.Functions.UnitTests.Tests
{
    public class GetAvailableServicesTest
    {
        [Fact]
        public async Task GetAvailableServices_return_expected_list_of_services()
        {
            var expectedResult = new CognitiveServicesListResult
            {
                StatusCode = HttpStatusCode.OK
            };

            var result = await GetAvailableServices.Run(new HttpRequestMessage(HttpMethod.Get, ""), TestFactory.CreateLogger(LoggerTypes.List));
            Assert.True(result is OkObjectResult);

            var resultDetails = result as OkObjectResult;
            Assert.True(resultDetails.StatusCode.Value == (int)HttpStatusCode.OK);

            Assert.True(((CognitiveServicesListResult)resultDetails.Value).AvaiableServices.Length == expectedResult.AvaiableServices.Length);
        }
    }
}
