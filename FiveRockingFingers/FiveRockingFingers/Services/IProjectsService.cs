using FiveRockingFingers.Entities;
using System.Collections.Generic;

namespace FiveRockingFingers.Services
{
    public interface IProjectsService
    {
        List<Project> GetAll();
    }
}