using System;

namespace FRF.Web.Dtos.Artifacts
{
    public class ArtifactsRelationDTO
    {
        public Guid Id { get; set; }
        public int Artifact1Id { get; set; }
        public ArtifactDTO Artifact1 { get; set; }
        public int Artifact2Id { get; set; }
        public ArtifactDTO Artifact2 { get; set; }        
        public string Artifact1Property { get; set; }
        public string Artifact2Property { get; set; }
        public int RelationTypeId { get; set; }
    }
}
