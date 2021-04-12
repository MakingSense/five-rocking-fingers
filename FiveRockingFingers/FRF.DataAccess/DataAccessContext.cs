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
        public DbSet<ConfigurationSettings> ConfigurationSettings { get; set; }
        public DbSet<UsersByProject> UsersByProject { get; set; }
        public DbSet<Artifact> Artifacts { get; set; }
        public DbSet<ArtifactType> ArtifactType { get; set; }
        public DbSet<ArtifactsRelation> ArtifactsRelation { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Resources> Resources { get; set; }

        public DataAccessContext(DbContextOptions<DataAccessContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration; 
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProjectCategory>().HasKey(pc => new { pc.ProjectId, pc.CategoryID });
            builder.Entity<ArtifactType>().HasKey(at => new { at.Id });
            builder.Entity<UsersByProject>().HasKey(up => new {up.Id});
            builder.Entity<UsersByProject>().HasOne(up=>up.Project).WithMany(u=>u.UsersByProject).HasForeignKey(up=>up.ProjectId);
            builder.Entity<ArtifactsRelation>().HasKey(ar => new {ar.Id});
            builder.Entity<Resources>().HasKey(re => new {re.Id});
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("OnConfiguring"));
        }
    }
}