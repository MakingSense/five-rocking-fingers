using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.Projects
{
    public class ProjectCategoryDTO
    {
        public CategoryDTO Category { get; set; }
        public ProjectDto Project { get; set; }
    }
}
