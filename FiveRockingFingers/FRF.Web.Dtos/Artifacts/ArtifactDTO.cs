using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace FRF.Web.Dtos.Artifacts
{
    public class ArtifactDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public XElement Settings { get; set; }
        public ProjectDto Project { get; set; }
        public ArtifactTypeDTO ArtifactType { get; set; }
    }
}
