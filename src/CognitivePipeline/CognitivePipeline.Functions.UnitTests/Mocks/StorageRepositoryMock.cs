using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.UnitTests.Mocks
{
    public class StorageRepositoryMock : IStorageRepository
    {
        public Task<string> CreateFileAsync(string name, byte[] fileData)
        {
            return Task.FromResult<string>(name);
        }

        public Task<string> CreateFileAsync(string name, Stream fileData)
        {
            return Task.FromResult<string>(name);
        }

        public Task<string> CreateFileAsync(string containerName, string fileName, Stream fileData)
        {
            return Task.FromResult<string>(fileName);
        }

        public Task<byte[]> GetFileAsync(string fileName)
        {
            return Task.FromResult<byte[]>(TestFactory.CognitiveFileBytes[0]);
        }

        public Task<byte[]> GetFileAsync(string containerName, string fileName)
        {
            return Task.FromResult<byte[]>(TestFactory.CognitiveFileBytes[0]);
        }

        public string GetFileDownloadUrl(string fileName)
        {
            return fileName;
        }
    }
}
