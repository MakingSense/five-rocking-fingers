using FRF.Web.Dtos.Projects;
using System;
using System.Collections.Generic;

namespace FRF.Web.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        #nullable enable
        public string? Name { get; set; }
        public string? Owner { get; set; }
        public string? Client { get; set; }
        public int? Budget { get; set; }
        #nullable disable
        public IList<ProjectCategoryDTO> ProjectCategories { get; set; }

    }
}
