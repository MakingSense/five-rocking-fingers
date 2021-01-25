using FRF.Core.Response;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IArtifactsService _artifactsService;

        public BudgetService(IArtifactsService artifactsService)
        {
            _artifactsService = artifactsService;
        }

        public async Task<ServiceResponse<decimal>> GetBudget(int projectId)
        {
            var budget = 0m;

            var result = await _artifactsService.GetAllByProjectId(projectId);

            if(!result.Success) return new ServiceResponse<decimal>(new Error(ErrorCodes.ProjectNotExists, result.Error.Message));

            foreach (var artifact in result.Value)
            {
                budget += artifact.GetPrice();
            }

            return new ServiceResponse<decimal>(budget);
        }
    }
}
