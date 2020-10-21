using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FRF.DataAccess.EntityModels
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        #nullable enable
        public string? Description { get; set; }
        #nullable disable
        public IList<ProjectCategory> ProjectCategories { get; set; }
    }
}
