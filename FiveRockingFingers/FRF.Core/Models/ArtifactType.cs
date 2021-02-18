using System.Collections.Generic;

namespace FRF.Core.Models
{
    public class ArtifactType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Artifact> Artifacts { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
        public string RequiredFields { get; set; }
    }
}
