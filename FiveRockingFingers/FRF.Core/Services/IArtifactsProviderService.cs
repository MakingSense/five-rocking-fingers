using FRF.Core.Models;
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
        Task<List<ProviderArtifactSetting>> GetAttributes(string serviceCode);
        Task<List<PricingTerm>> GetProducts(List<KeyValuePair<string, string>> settings, string serviceCode);
    }
}