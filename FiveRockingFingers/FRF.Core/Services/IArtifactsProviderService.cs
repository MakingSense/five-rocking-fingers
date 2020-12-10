using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IArtifactsProviderService
    {
        Task<List<KeyValuePair<string, string>>> GetAllNamesAsync();
    }
}