using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FRF.Core.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Owner { get; set; }
        public string? Client { get; set; }
        [Required]
        [Range(1, 1000000000000 , ErrorMessage = "Budget have to be greater than 0")]
        public int? Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public IList<ProjectCategory> ProjectCategories { get; set; }
        public ICollection<UsersByProject> UsersByProject { get; set; }
    }
}
