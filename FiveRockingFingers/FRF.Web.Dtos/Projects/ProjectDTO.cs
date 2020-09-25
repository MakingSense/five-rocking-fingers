using System;

namespace FRF.Web.Dtos
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Client { get; set; }
        public int Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CategoryId { get; set; }
        public ProjectCategoryDTO ProjectCategoryDTO { get; set; }
        public bool isActive { get; set; }
        public DateTime StartDate { get; set; }

    }

    public class ProjectCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
