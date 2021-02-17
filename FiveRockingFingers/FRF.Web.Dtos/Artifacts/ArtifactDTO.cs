using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FRF.Web.Dtos.Artifacts
{
    public class ArtifactDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The artifact needs a name")]
        public string Name { get; set; }
        public Dictionary<string, string> Settings { get; set; }
        public int ProjectId { get; set; }
        public ArtifactTypeDTO ArtifactType { get; set; }
        public decimal Price { get; set; }
    }
}
