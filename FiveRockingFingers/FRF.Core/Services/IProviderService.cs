using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface IProviderService
    {
        Task<IList> GetAllAsync();
        Task<List<KeyValuePair<string, string>>> GetAllNamesAsync();
        Task<T> GetAsync<T>(string serviceCode);
        Task<T> GetAsync<T>(string serviceCode,string nextToken);
    }
}