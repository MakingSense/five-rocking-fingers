using FRF.Core.Models;
using System.Collections.Generic;

namespace FRF.Core.Services
{
    public interface IArtifactsService
    {
        List<Artifact> GetAll();
        Artifact Get(int id);
        Artifact Update(Artifact artifact);
        void Delete(int id);
        Artifact Save(Artifact artifact);
    }
}
