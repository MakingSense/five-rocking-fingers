using System.ComponentModel.DataAnnotations;

namespace FRF.DataAccess.EntityModels
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
