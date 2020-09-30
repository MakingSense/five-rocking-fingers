using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Models
{
    public class Project
    {
        public int Id { get; set; }
        #nullable enable
        public string? Name { get; set; }
        public string? Owner { get; set; }
        public string? Client { get; set; }
        public int? Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        #nullable disable
        public IList<ProjectCategory> ProjectCategories { get; set; }
    }
}
