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
        #nullable enable
        public string? SettingsXML { get; set; }
        #nullable disable
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int ArtifactTypeId { get; set; }
        public ArtifactType ArtifactType { get; set; }
        [NotMapped]
        public XElement Settings
        {
            get => _ = SettingsXML != null ? XElement.Parse(SettingsXML) : null;
            set { SettingsXML = value?.ToString(); }
        }
    }
}

