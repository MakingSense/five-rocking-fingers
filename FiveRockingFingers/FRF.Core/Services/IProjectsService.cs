using FRF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IProjectsService
    {
        List<Project> GetAll();
        Task<List<Project>> GetAllByUserId(string userId);
        Project Get(int id);
        Project Update(Project project);
        void Delete(int id);
        Project Save(Project project);
    }
}