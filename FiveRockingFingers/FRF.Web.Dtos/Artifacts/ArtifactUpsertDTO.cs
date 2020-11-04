using System.Xml.Linq;

namespace FRF.Web.Dtos.Artifacts
{
    public class ArtifactUpsertDTO
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public XElement Settings { get; set; }
        public int ProjectId { get; set; }
        public int ArtifactTypeId { get; set; }
    }
}
