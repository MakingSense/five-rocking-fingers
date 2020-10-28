using FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FRF.DataAccess
{
    public class DataAccessContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Project> Projects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<ArtifactType> ArtifactType { get; set; }

        public DataAccessContext(DbContextOptions<DataAccessContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration; 
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProjectCategory>().HasKey(pc => new { pc.ProjectId, pc.CategoryID });
            builder.Entity<ArtifactType>().HasKey(at => new { at.Id });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("OnConfiguring"));
        }
    }
}