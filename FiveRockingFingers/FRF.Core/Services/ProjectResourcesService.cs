using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class ProjectResourcesService : IProjectResourcesService
    {
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ProjectResourcesService(DataAccessContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<ProjectResource>> GetAsync(int id)
        {
            var projectResource = await _dataContext.ProjectResources.SingleOrDefaultAsync(pr => pr.Id == id);

            if (projectResource == null)
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ProjectResourceNotExists, $"There is no project resource with Id = {id}"));

            var mappedProjectResource = _mapper.Map<ProjectResource>(projectResource);
            return new ServiceResponse<ProjectResource>(mappedProjectResource);
        }

        public async Task<ServiceResponse<List<ProjectResource>>> GetByProjectIdAsync(int projectId)
        {
            if (!await _dataContext.Projects.AnyAsync(p => p.Id == projectId))
                return new ServiceResponse<List<ProjectResource>>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {projectId}"));

            var projectResources = await _dataContext.ProjectResources
                .Where(pr => pr.ProjectId == projectId)
                .ToListAsync();

            var mappedProjectResources = _mapper.Map<List<ProjectResource>>(projectResources);

            return new ServiceResponse<List<ProjectResource>>(mappedProjectResources);
        }

        public async Task<ServiceResponse<ProjectResource>> SaveAsync(ProjectResource projectResource)
        {
            var project = await _dataContext.Projects.SingleOrDefaultAsync(p => p.Id == projectResource.ProjectId);

            if (project == null)
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {projectResource.ProjectId}"));

            if (!await _dataContext.Resources.AnyAsync(r => r.Id == projectResource.ResourceId))
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ResourceNotExists, $"There is no resource with Id = {projectResource.ResourceId}"));

            if (!IsStartDateValid(project.StartDate, projectResource.BeginDate))
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.InvalidBeginDateForProjectResource, $"The begin date of the project resource cannot be before the start date of the project"));
            
            if (!IsEndDateValid(project.StartDate, projectResource.BeginDate, projectResource.EndDate))
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.InvalidEndDateForProjectResource, $"The end date of the project resource cannot be before the begin date or the start date of the project"));

            // Maps the category into an EntityModel, deleting the Id if there was one.
            var mappedProjectResource = _mapper.Map<DataAccess.EntityModels.ProjectResource>(projectResource);

            // Adds the category to the database, generating a unique Id for it
            await _dataContext.ProjectResources.AddAsync(mappedProjectResource);

            // Saves changes
            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<ProjectResource>(_mapper.Map<ProjectResource>(mappedProjectResource));
        }

        public async Task<ServiceResponse<ProjectResource>> UpdateAsync(ProjectResource projectResource)
        {
            var projectResourseResponse = await GetAsync(projectResource.Id);

            if(!projectResourseResponse.Success)
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ProjectResourceNotExists, $"There is no project with Id = {projectResource.ProjectId}"));

            var project = await _dataContext.Projects.SingleOrDefaultAsync(p => p.Id == projectResource.ProjectId);

            if (project == null)
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ProjectNotExists, $"There is no project with Id = {projectResource.ProjectId}"));

            if (!await _dataContext.Resources.AnyAsync(r => r.Id == projectResource.ResourceId))
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.ResourceNotExists, $"There is no resource with Id = {projectResource.ResourceId}"));

            if (!IsStartDateValid(project.StartDate, projectResource.BeginDate))
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.InvalidBeginDateForProjectResource, $"The begin date of the project resource cannot be before the start date of the project"));

            if (!IsEndDateValid(project.StartDate, projectResource.BeginDate, projectResource.EndDate))
                return new ServiceResponse<ProjectResource>(new Error(ErrorCodes.InvalidEndDateForProjectResource, $"The end date of the project resource cannot be before the begin date or the start date of the project"));

            var result = await _dataContext.ProjectResources.SingleAsync(pr => pr.Id == projectResource.Id);
            result.BeginDate = projectResource.BeginDate;
            result.EndDate = projectResource.EndDate;
            result.DedicatedHours = projectResource.DedicatedHours;
            result.ResourceId = projectResource.ResourceId;

            await _dataContext.SaveChangesAsync();

            var mappedProjectResource = _mapper.Map<ProjectResource>(result);
            return new ServiceResponse<ProjectResource>(mappedProjectResource);
        }

        public async Task<ServiceResponse<ProjectResource>> DeleteAsync(int id)
        {
            var projectResourceToDelete = await _dataContext.ProjectResources.SingleAsync(pr => pr.Id == id);
            _dataContext.ProjectResources.Remove(projectResourceToDelete);
            await _dataContext.SaveChangesAsync();

            var mappedProjectResource = _mapper.Map<ProjectResource>(projectResourceToDelete);
            return new ServiceResponse<ProjectResource>(mappedProjectResource);
        }

        private bool IsStartDateValid(DateTime? startDateOfProject, DateTime? beginDate)
        {
            if (startDateOfProject == null && beginDate != null)
                return false;

            if (beginDate == null)
                return true;

            return startDateOfProject?.Date <= beginDate?.Date;
        }

        private bool IsEndDateValid(DateTime? startDateOfProject, DateTime? beginDate, DateTime? endDate)
        {
            if (startDateOfProject == null && endDate != null)
                return false;

            if (endDate == null)
                return true;

            if(beginDate != null)
                return beginDate?.Date < endDate?.Date;

            return startDateOfProject?.Date < endDate?.Date;
        }
    }
}