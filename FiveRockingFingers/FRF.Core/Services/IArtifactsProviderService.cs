using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    /// <summary>
    /// A base interface for different artifacts providers.
    /// </summary>
    public interface IArtifactsProviderService
    {
        Task<List<KeyValuePair<string, string>>> GetNamesAsync();
    }
}