using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FRF.Core.Models
{
    public class Artifact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public XElement Settings { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int ArtifactTypeId { get; set; }
        public ArtifactType ArtifactType { get; set; }
    }
}