using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace FRF.Core.Models
{
    public class Artifact : IArtifactPricing
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public XElement Settings { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int ArtifactTypeId { get; set; }
        public ArtifactType ArtifactType { get; set; }
        public Dictionary<string, string> RelationalFields { get; set; }

        public virtual decimal GetPrice()
        {
            return 0;
        }
    }
}