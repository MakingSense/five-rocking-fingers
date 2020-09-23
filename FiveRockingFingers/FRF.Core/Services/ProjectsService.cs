using FRF.Core.Models;
using FRF.DataAccess;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace FRF.Core.Services
{
    public class ProjectsService : IProjectsService
    {
        public IConfiguration Configuration { get; set; }
        public DataAccessContext DataContext { get; set; }
        public ProjectsService(IConfiguration configuration, DataAccessContext dataContext)
        {
            Configuration = configuration;
            DataContext = dataContext;
        }

        public List<Project> GetAll()
        {
            //Create Mapper
            DataContext.Projects.ToList();
            return new List<Project>(); 
        }

        public Project Save(Project project)
        {
            //Add mapper

            var result = DataContext.Projects.Add(new DataAccess.EntityModels.Project());
            DataContext.SaveChanges();
            return project;

        }
    }
}
