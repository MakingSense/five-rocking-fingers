using FiveRockingFingers.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiveRockingFingers
{
    public class DataAccessContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        public DataAccessContext(DbContextOptions<DataAccessContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }
    }
}
