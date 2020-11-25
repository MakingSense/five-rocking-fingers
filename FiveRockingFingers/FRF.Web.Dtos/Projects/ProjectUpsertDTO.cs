using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos.Projects
{
    public class ProjectUpsertDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Owner { get; set; }
        [Required]
        public string? Client { get; set; }
        [Required]
        [Range(1, 1000000000000, ErrorMessage = "Budget have to be greater than {1}")]
        public int? Budget { get; set; }
        public IList<ProjectCategoryDTO> ProjectCategories { get; set; }
        public IList<UsersByProjectDTO> UsersByProject { get; set; }
    }
}
