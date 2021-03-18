using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;

namespace FRF.Core.Tests
{
    public class DataAccessContextForTest : DataAccessContext
    {
        private readonly IConfiguration _configuration;

        public DataAccessContextForTest(Guid dbGuid, IConfiguration configuration) : base(
            new DbContextOptionsBuilder<DataAccessContext>().UseInMemoryDatabase(databaseName: dbGuid.ToString()).ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options, configuration)

        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
