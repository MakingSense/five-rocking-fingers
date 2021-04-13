using System;
using System.Collections.Generic;

namespace FRF.Core.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Owner { get; set; }
        public string? Client { get; set; }
        public int? Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public IList<ProjectCategory> ProjectCategories { get; set; }
        public ICollection<UsersProfile> UsersByProject { get; set; }
        public IList<ProjectResources> ProjectResources { get; set; }
    }
}