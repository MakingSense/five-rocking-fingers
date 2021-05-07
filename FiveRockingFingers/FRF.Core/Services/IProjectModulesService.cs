using FRF.Core.Models;
using FRF.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IProjectModulesService
    {
        public Task<ServiceResponse<ProjectModule>> GetAsync(int id);
        public Task<ServiceResponse<List<ProjectModule>>> GetByProjectIdAsync(int projectId);
        public Task<ServiceResponse<ProjectModule>> SaveAsync(ProjectModule projectModule);
        public Task<ServiceResponse<ProjectModule>> UpdateAsync(ProjectModule projectModule);
        public Task<ServiceResponse<ProjectModule>> DeleteAsync(int id);
    }
}
