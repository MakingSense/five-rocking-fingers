namespace FRF.DataAccess.EntityModels
{
    public class CategoryModule
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
    }
}