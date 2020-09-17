using FiveRockingFingers.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FiveRockingFingers.Services
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
            return DataContext.Projects.ToList();
        }

        public Project Save(Project project)
        {
            var result = DataContext.Projects.Add(project);
            DataContext.SaveChanges();
            return project;

        }
    }
}
}
