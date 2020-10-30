using FRF.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ICategoriesService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category> GetAsync(int id);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(int id);
        Task<Category> SaveAsync(Category category);
    }
}
