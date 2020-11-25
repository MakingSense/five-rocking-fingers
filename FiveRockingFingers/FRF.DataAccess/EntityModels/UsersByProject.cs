using System;
using System.ComponentModel.DataAnnotations;

namespace FRF.DataAccess.EntityModels
{
    public class UsersByProject
    {
        [Key]
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public Guid UserId { get; set; }
    }
}
