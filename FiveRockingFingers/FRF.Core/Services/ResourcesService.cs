using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class ResourcesService : IResourcesService
    {
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ResourcesService(DataAccessContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<Resource>> GetAsync(int id)
        {
            var response = await _dataContext.Resources
                .Include(r => r.ProjectResource)
                .ThenInclude(pr => pr.Project)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (response == null)
                return new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNotExist,
                    $"There is not resource with Id = {id}"));

            var resource = _mapper.Map<Resource>(response);
            return new ServiceResponse<Resource>(resource);
        }

        public async Task<ServiceResponse<Resource>> UpdateAsync(Resource resource)
        {
            var originalResource = await _dataContext.Resources
                .FirstOrDefaultAsync(r => r.Id == resource.Id);
            if (originalResource == null)
                return new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNotExist,
                    $"There is not resource with Id = {resource.Id}"));

            var isRoleNameRepeated = await IsRoleNameRepeated(resource, isUpdate: true);
            if (isRoleNameRepeated)
                return new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNameRepeated,
                    $"The role name = \"{resource.RoleName}\" already exist."));

            originalResource.RoleName = resource.RoleName;
            originalResource.Description = resource.Description;
            originalResource.SalaryPerMonth = resource.SalaryPerMonth;
            originalResource.WorkloadCapacity = resource.WorkloadCapacity;
            await _dataContext.SaveChangesAsync();

            var updatedResource = _mapper.Map<Resource>(originalResource);
            return new ServiceResponse<Resource>(updatedResource);
        }

        public async Task<ServiceResponse<Resource>> DeleteAsync(int id)
        {
            var resourceToDelete = await _dataContext.Resources.FirstOrDefaultAsync(r => r.Id == id);
            if (resourceToDelete == null)
                return new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNotExist,
                    $"There is not resource with Id = {id}"));
            _dataContext.Resources.Remove(resourceToDelete);
            await _dataContext.SaveChangesAsync();

            var deletedResource = _mapper.Map<Resource>(resourceToDelete);
            return new ServiceResponse<Resource>(deletedResource);
        }

        public async Task<ServiceResponse<Resource>> SaveAsync(Resource resource)
        {
            var isRoleNameRepeated = await IsRoleNameRepeated(resource, isUpdate: false);
            if (isRoleNameRepeated)
                return new ServiceResponse<Resource>(new Error(ErrorCodes.ResourceNameRepeated,
                    $"The role name = ' {resource.RoleName} ' already exist."));
            var resourceToSave = _mapper.Map<DataAccess.EntityModels.Resource>(resource);
            await _dataContext.Resources.AddAsync(resourceToSave);
            await _dataContext.SaveChangesAsync();

            var newResource = await _dataContext.Resources.FirstOrDefaultAsync(r => r.Id == resourceToSave.Id);
            return new ServiceResponse<Resource>(_mapper.Map<Resource>(newResource));
        }

        private async Task<bool> IsRoleNameRepeated(Resource resource, bool isUpdate)
        {
            return isUpdate ? 
                await _dataContext.Resources.AnyAsync(r => r.Id != resource.Id && r.RoleName == resource.RoleName) 
                :
                await _dataContext.Resources.AnyAsync(r => r.RoleName == resource.RoleName);
        }
    }
}