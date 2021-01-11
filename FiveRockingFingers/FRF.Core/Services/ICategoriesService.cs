using FRF.Core.Models;
using FRF.Core.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ICategoriesService
    {
        Task<ServiceResponse<List<Category>>> GetAllAsync();
        Task<ServiceResponse<Category>> GetAsync(int id);
        Task<ServiceResponse<Category>> UpdateAsync(Category category);
        Task<ServiceResponse<Category>> DeleteAsync(int id);
        Task<ServiceResponse<Category>> SaveAsync(Category category);
    }
}
