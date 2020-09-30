using FRF.Core.Base;

namespace FRF.Core.Services
{
    public interface IConfigurationService
    {
        CognitoConfigurationBase GetConfigurationSettings();
    }
}
