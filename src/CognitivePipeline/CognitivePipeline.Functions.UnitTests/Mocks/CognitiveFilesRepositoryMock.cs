using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.Models;
using CognitivePipeline.Functions.UnitTests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.UnitTests.Mocks
{
    public class CognitiveFilesRepositoryMock : ICognitiveFilesRepository
    {
        public Task<CognitiveFile> AddAsync(CognitiveFile entity)
        {
            return Task.FromResult<CognitiveFile>(entity);
        }

        public Task DeleteAsync(CognitiveFile entity)
        {
            return Task.CompletedTask;
        }

        public Task<CognitiveFile> GetByIdAsync(string id)
        {
            return Task.FromResult<CognitiveFile>(new CognitiveFile());
        }

        public Task UpdateAsync(CognitiveFile entity)
        {
            return Task.CompletedTask;
        }
    }
}
