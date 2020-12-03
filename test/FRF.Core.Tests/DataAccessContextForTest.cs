using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FRF.Core.Tests
{
    public class DataAccessContextForTest : DataAccessContext
    {
        private readonly IConfiguration _configuration;
        public DataAccessContextForTest(DbContextOptions<DataAccessContextForTest> options, IConfiguration configuration) : base(
            new DbContextOptionsBuilder<DataAccessContext>().UseInMemoryDatabase(databaseName: "Test").Options, configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
