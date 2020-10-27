using FRF.DataAccess.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace FRF.DataAccess
{
    public class DataAccessContext : DbContext
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<ConfigurationSettings> ConfigurationSettings { get; set; }
        public DbSet<UsersByProject> UsersByProject { get; set; }

        public DataAccessContext(DbContextOptions<DataAccessContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProjectCategory>().HasKey(pc => new { pc.ProjectId, pc.CategoryID });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-28EG8E0;database=FiveRockingFingers;trusted_connection=true;");
        }
    }
}
