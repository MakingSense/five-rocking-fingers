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
        public DataAccessContext Context { get; set; }
        public ProjectsService(IConfiguration configuration, DataAccessContext context)
        {
            Configuration = configuration;
            Context = context;
        }

        public List<Project> GetAll()
        {
            return Context.Projects.ToList();
        }
    }
}
