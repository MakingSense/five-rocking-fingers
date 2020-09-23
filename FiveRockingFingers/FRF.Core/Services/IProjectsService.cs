using FRF.Core.Models;
using System.Collections.Generic;

namespace FRF.Core.Services
{
    public interface IProjectsService
    {
        List<Project> GetAll();
    }
}