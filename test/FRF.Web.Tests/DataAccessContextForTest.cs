using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace FRF.Web.Tests
{
    public class DataAccessContextForTest : DataAccessContext
    {
        private readonly IConfiguration _configuration;

        public DataAccessContextForTest(Guid dbGuid, IConfiguration configuration) : base(
            new DbContextOptionsBuilder<DataAccessContext>().UseInMemoryDatabase(databaseName: dbGuid.ToString()).Options, configuration)

        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
