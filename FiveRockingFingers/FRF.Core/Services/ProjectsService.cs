using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace FRF.Core.Services
{
    public class ProjectsService : IProjectsService
    {
        public IConfiguration Configuration { get; set; }
        public DataAccessContext DataContext { get; set; }
        private readonly IMapper Mapper;
        public ProjectsService(IConfiguration configuration, DataAccessContext dataContext, IMapper mapper)
        {
            Configuration = configuration;
            DataContext = dataContext;
            Mapper = mapper;
        }

        public List<Project> GetAll()
        {
            var result = DataContext.Projects.Include(p => p.ProjectCategories).ThenInclude(pc => pc.Category).ToList();
            return Mapper.Map<List<Project>>(result); 
        }

        public Project Save(Project project)
        {
            //Add mapper
            var result = DataContext.Projects.Add(new DataAccess.EntityModels.Project());
            DataContext.SaveChanges();
            return project;

        }

        public Project Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public Project Update(Project project)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
