using System;
using System.Collections.Generic;

namespace FRF.Web.Dtos.Projects
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string? Client { get; set; }
        public int? Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<ProjectCategoryDTO> ProjectCategories { get; set; }
        public IList<UserByProjectViewModel> UsersByProject { get; set; }
    }
}