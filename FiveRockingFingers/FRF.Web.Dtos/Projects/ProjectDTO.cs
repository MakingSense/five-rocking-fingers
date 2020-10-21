using FRF.Web.Dtos.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FRF.Web.Dtos
{
    public class ProjectDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"^([\da-zA-Z]{8}-([\da-zA-Z]{4}-){3}[\da-zA-Z]{12}$)")]
        public int UserId { get; set; }
        #nullable enable
        public string? Name { get; set; }
        public string? Owner { get; set; }
        public string? Client { get; set; }
        public int? Budget { get; set; }
        public DateTime CreatedDate { get; set; }
#nullable disable
        public IList<ProjectCategoryDTO> ProjectCategories { get; set; }

    }
}
