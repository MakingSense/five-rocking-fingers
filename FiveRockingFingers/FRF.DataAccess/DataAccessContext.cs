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
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProjectResource> ProjectResources { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<ProjectModule> ProjectModules { get; set; }
        public DbSet<CategoryModule> CategoriesModules { get; set; }

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
            builder.Entity<Resource>().HasKey(re => new {re.Id});
            builder.Entity<ProjectResource>().HasKey(pr => new { pr.Id});
            builder.Entity<Module>().HasKey(m => new { m.Id});
            builder.Entity<ProjectModule>().HasKey(pm => new { pm.Id});
            builder.Entity<CategoryModule>().HasKey(cm => new { cm.CategoryId, cm.ModuleId});
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("OnConfiguring"));
        }
    }
}