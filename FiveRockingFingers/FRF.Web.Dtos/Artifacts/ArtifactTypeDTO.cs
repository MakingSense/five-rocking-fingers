namespace FRF.Web.Dtos.Artifacts
{
    public class ArtifactTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProviderDTO Provider { get; set; }
        public string RequiredFields { get; set; }
    }
}
