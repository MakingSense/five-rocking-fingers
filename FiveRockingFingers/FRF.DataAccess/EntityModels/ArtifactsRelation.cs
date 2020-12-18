namespace FRF.DataAccess.EntityModels
{
    public class ArtifactsRelation
    {
        public int Artifact1Id { get; set; }
        public Artifact Artifact1 { get; set; }

        public int Artifact2Id { get; set; }
        public Artifact Artifact2 { get; set; }

        public string Artifact1Property { get; set; }
        public string Artifact2Property { get; set; }
        public int RelationTypeId { get; set; }
    }
}