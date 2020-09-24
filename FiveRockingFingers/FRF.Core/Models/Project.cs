using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Client { get; set; }
        public int Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ProjectCategoryId { get; set; }
        public ProjectCategory ProjectCategory { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class ProjectCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
