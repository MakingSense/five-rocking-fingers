using System.Collections.Generic;

namespace FRF.Core.Models
{
    public class Provider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ArtifactType> ArtifactType { get; set; }
    }
}
