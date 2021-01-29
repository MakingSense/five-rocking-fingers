using FRF.Core.Response;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IBudgetService
    {
        Task<ServiceResponse<decimal>> GetBudget(int projectId);
    }
}
