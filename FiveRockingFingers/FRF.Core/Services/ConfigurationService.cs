using FRF.Core.Base;
using FRF.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace FRF.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly DataAccessContext _dataContext;

        public ConfigurationService(DataAccessContext dataContext)
        {
            _dataContext = dataContext;
        }

        /* TODO:Pending AWS Credentials. Login is bypassed![FIVE-6] */
        /*Uncomment this after do.
         public CognitoConfigurationBase GetConfigurationSettings()
        {
            var resultFromDb = _dataContext.ConfigurationSettings.ToDictionary(k => k.Name, v => v.Value);
                var result = ToObject<CognitoConfigurationBase>(resultFromDb);
                return result ?? null;
        }*/

        public static T ToObject<T>(IDictionary<string, string> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key)
                    ?.SetValue(someObject, item.Value, null);
            }

            return someObject;
        }
    }
}