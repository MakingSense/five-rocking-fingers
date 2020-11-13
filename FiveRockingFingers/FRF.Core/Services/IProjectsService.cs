using FRF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IProjectsService
    {
        Task<List<Project>> GetAllAsync(string userId);
        Task<Project> GetAsync(int id);
        Task<Project> UpdateAsync(Project project);
        Task<bool> DeleteAsync(int id);
        Task<Project> SaveAsync(Project project);
        bool IsAuthorized(Project project, string userId);
    }
}