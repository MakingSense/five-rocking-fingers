using FRF.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FRF.Core.Services
{
    public interface ICategoriesService
    {
        Task<List<Category>> GetAll();
        Task<Category> Get(int id);
        Task<Category> Update(Category category);
        Task Delete(int id);
        Task<Category> Save(Category category);
    }
}
