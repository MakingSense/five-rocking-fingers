using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace FRF.Web.Dtos.Artifacts
{
    public class ArtifactDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The artifact needs a name")]
        public string Name { get; set; }
        public string Provider { get; set; }
        public XElement Settings { get; set; }
        public int ProjectId { get; set; }
        public ArtifactTypeDTO ArtifactType { get; set; }
    }
}
