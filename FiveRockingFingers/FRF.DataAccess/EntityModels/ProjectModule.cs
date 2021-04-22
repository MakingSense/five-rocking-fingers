namespace FRF.DataAccess.EntityModels
{
    public class ProjectModule
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public string Alias { get; set; }
        public int Cost { get; set; }
    }
}