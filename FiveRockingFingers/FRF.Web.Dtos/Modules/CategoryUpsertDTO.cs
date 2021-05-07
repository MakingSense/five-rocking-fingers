using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Modules
{
    public class CategoryUpsertDTO
    {
        [Required]
        public int Id { get; set; }
    }
}