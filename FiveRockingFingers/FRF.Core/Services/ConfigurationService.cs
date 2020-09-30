using FRF.Core.Base;
using FRF.DataAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FRF.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public DataAccessContext DataContext { get; set; }

        public ConfigurationService(DataAccessContext dataContext)
        {
            DataContext = dataContext;
        }

        
        public CognitoConfigurationBase GetConfigurationSettings()
        {
            try
            {
                var resultFromDb = DataContext.ConfigurationSettings.ToDictionary(k => k.Name, v => v.Value);
                var result = ToObject<CognitoConfigurationBase>(resultFromDb);
                return result ?? null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

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