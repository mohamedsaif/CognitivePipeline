using CognitivePipeline.Functions.Abstractions;
using CognitivePipeline.Functions.Models;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitivePipeline.Functions.Data
{
    public class UserAccountRepository : CosmosDbRepository<UserAccount>, IUserAccountRepository
    {
        public UserAccountRepository(ICosmosDbClientFactory factory) : base(factory) { }

        public override string CollectionName { get; } = AppConstants.DbUserAccountsContainer;
        public override string GenerateId(UserAccount entity) => $"{entity.AccountId}:{Guid.NewGuid()}";
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId.Split(':')[0]);
    }

}
