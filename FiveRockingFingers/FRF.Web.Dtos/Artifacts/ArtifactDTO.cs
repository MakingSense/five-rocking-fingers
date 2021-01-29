using FRF.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System;
using System.Collections.Generic;

namespace FRF.Web.Dtos.Artifacts
{
    public class ArtifactDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The artifact needs a name")]
        public string Name { get; set; }
        public string Provider { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public int ProjectId { get; set; }
        public ArtifactTypeDTO ArtifactType { get; set; }
    }
}
