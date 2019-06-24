using CognitivePipeline.Functions.Functions;
using CognitivePipeline.Functions.UnitTests.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CognitivePipeline.Functions.UnitTests.Tests
{
    public class NewCognitiveReqTest
    {
        [Fact]
        public async Task NewCogntiveReq_http_trigger_return_successful_exection()
        {
            var function = new NewCognitiveReq(
                TestFactory.CreateCognitiveFilesRepositoryMock(), 
                TestFactory.CreateUserAccountsRepositoryMock(), 
                TestFactory.CreateStorageRepositoryMock(), 
                TestFactory.CreateQueueRepositoryMock());

            var result = await function.Run(TestFactory.GetMultipartRequest(0), TestFactory.CreateLogger(LoggerTypes.List));

            Assert.True(result is CreatedResult);
        }
    }
}
