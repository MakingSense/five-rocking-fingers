using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FRF.Web.Dtos.Projects
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        #nullable enable
        public string? Description { get; set; }
        #nullable disable
        public IList<ProjectCategoryDTO> ProjectCategories { get; set; }
    }
}
