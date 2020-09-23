﻿using FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace FRF.DataAccess
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
