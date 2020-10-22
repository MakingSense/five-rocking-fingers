using FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace FRF.DataAccess
{
    public class DataAccessContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<ArtifactType> ArtifactType { get; set; }

        public DataAccessContext(DbContextOptions<DataAccessContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProjectCategory>().HasKey(pc => new { pc.ProjectId, pc.CategoryID });
            builder.Entity<ArtifactType>().HasKey(at => new { at.Id });
        }
    }
}
