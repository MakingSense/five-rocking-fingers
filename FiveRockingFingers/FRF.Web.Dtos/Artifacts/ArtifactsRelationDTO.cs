using System;
using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Artifacts
{
    public class ArtifactsRelationDTO
    {
        [Required] 
        public Guid? Id { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        public int Artifact1Id { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Artifact2Id { get; set; }
        [Required]
        public string Artifact1Property { get; set; }
        [Required]
        public string Artifact2Property { get; set; }
        [Required]
        [Range(0, 2)]
        public int RelationTypeId { get; set; }
    }
}
