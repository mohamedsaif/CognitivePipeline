using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.UnitTests.Mocks
{
    public class UserAccountsRepository : IUserAccountRepository
    {
        public Task<UserAccount> AddAsync(UserAccount entity)
        {
            return Task.FromResult<UserAccount>(entity);
        }

        public Task DeleteAsync(UserAccount entity)
        {
            return Task.CompletedTask;
        }

        public Task<UserAccount> GetByIdAsync(string id)
        {
            return Task.FromResult<UserAccount>(new UserAccount());
        }

        public Task UpdateAsync(UserAccount entity)
        {
            return Task.CompletedTask;
        }
    }
}
