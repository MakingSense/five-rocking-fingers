using FRF.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Services
{
    public interface ICategoryService
    {
        List<Category> GetAll();
        Category Get(int id);
        Category Update(Category category);
        void Delete(int id);
        Category Save(Category category);
    }
}
