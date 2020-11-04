using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FRF.Web.Dtos.Categories
{
    public class CategoryUpsertDTO
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        #nullable enable
        public string? Description { get; set; }
        #nullable disable
    }
}
