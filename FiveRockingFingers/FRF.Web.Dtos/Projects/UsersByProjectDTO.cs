using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Projects
{
    public class UsersByProjectDTO
    {
        [CustomValidator.UserId]
        public string UserId { get; set; }
        [Required]
        public int ProjectId { get; set; }
    }
}
