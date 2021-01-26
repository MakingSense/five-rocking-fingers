using System;
using FRF.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using FRF.Core.Response;

namespace FRF.Core.Services
{
    public interface IProjectsService
    {
        Task<ServiceResponse<List<Project>>> GetAllAsync(Guid userId);
        Task<ServiceResponse<Project>> GetAsync(int id);
        Task<ServiceResponse<Project>> UpdateAsync(Project project);
        Task<ServiceResponse<Project>> DeleteAsync(int id);
        Task<ServiceResponse<Project>> SaveAsync(Project project);
    }
}