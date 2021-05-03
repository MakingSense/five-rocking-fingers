using FRF.Core.Models;
using FRF.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IProjectResourcesService
    {
        public Task<ServiceResponse<ProjectResource>> GetAsync(int id);
        public Task<ServiceResponse<List<ProjectResource>>> GetByProjectIdAsync(int projectId);
        public Task<ServiceResponse<ProjectResource>> SaveAsync(ProjectResource projectResource);
        public Task<ServiceResponse<ProjectResource>> UpdateAsync(ProjectResource projectResource);
        public Task<ServiceResponse<ProjectResource>> DeleteAsync(int id);
    }
}
