using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace FRF.DataAccess.EntityModels
{
    public class Artifact
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public string SettingsXML { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int ArtifactTypeId { get; set; }
        public ArtifactType ArtifactType { get; set; }
        [NotMapped]
        public XElement Settings
        {
            get { return XElement.Parse(SettingsXML); }
            set { SettingsXML = value.ToString(); }
        }
    }
}

