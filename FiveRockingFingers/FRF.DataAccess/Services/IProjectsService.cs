using FRF.Core.Entities;
using System.Collections.Generic;

namespace FRF.DataAccess.Services
{
    public interface IProjectsService
    {
        List<Project> GetAll();
    }
}