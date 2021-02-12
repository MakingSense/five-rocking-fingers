using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Response;
using FRF.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class ArtifactTypesService : IArtifactTypesService
    {
        private readonly DataAccessContext _dataContext;
        private readonly IMapper _mapper;

        public ArtifactTypesService(DataAccessContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<ArtifactType>>> GetAllAsync()
        {
            var artifactTypes = await _dataContext.ArtifactType
                .Include(at => at.Provider)
                .ToListAsync();

            var mappedArtifactTypes = _mapper.Map<List<ArtifactType>>(artifactTypes);
            return new ServiceResponse<List<ArtifactType>>(mappedArtifactTypes);
        }

        public async Task<ServiceResponse<List<ArtifactType>>> GetAllByProviderAsync(string providerName)
        {
            var trimmedName = providerName.Trim();

            if (!await _dataContext.Providers.AnyAsync(p => p.Name == trimmedName))
            {
                return new ServiceResponse<List<ArtifactType>>(new Error(ErrorCodes.ProviderNotExists, $"There is no '{trimmedName}' provider"));
            }

            var artifactTypes = await _dataContext.ArtifactType
                .Include(at => at.Provider)
                .Where(at => at.Provider.Name == trimmedName)
                .ToListAsync();

            var mappedArtifactTypes = _mapper.Map<List<ArtifactType>>(artifactTypes);
            return new ServiceResponse<List<ArtifactType>>(mappedArtifactTypes);
        }
    }
}
