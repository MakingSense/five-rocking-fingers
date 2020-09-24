using FRF.Core.Models;
using System.Collections.Generic;

namespace FRF.Core.Services
{
    public interface IProjectsService
    {
        List<Project> GetAll();
        Project Get(int id);
        Project Update(Project project);
        void Delete(int id);
        Project Save(Project project);
    }
}