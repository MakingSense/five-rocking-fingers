using FRF.Core.Models;
using FRF.Core.Response;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    /// <summary>
    /// A base interface for different artifacts providers.
    /// </summary>
    public interface IArtifactsProviderService
    {
        Task<ServiceResponse<List<KeyValuePair<string, string>>>> GetNamesAsync();
        Task<ServiceResponse<List<ProviderArtifactSetting>>> GetAttributesAsync(string serviceCode);
        Task<ServiceResponse<List<PricingTerm>>> GetProductsAsync(List<KeyValuePair<string, string>> settings, string serviceCode);
        Task<ServiceResponse<JObject>> GetRequireFildsAsync(string serviceCode);
    }
}