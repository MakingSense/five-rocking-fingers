using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FRF.DataAccess.EntityModels
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        #nullable enable
        public string? Owner { get; set; }
        public string? Client { get; set; }
        public int? Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate{ get; set; }
        public DateTime? StartDate { get; set; }
        #nullable disable
        public IList<ProjectCategory> ProjectCategories { get; set; }
        public ICollection<UsersByProject> UsersByProject { get; set; }
    }
}
