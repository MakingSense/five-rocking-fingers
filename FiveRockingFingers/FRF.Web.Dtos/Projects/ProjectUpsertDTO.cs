using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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
        public int? Budget { get; set; }
        public IList<ProjectCategoryDTO> ProjectCategories { get; set; }
        public IList<UsersByProjectDTO> UsersByProject { get; set; }
    }
}
