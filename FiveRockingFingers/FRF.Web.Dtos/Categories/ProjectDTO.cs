using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.Categories
{
    public class ProjectDTO
    {
        public int Id { get; set; }
        #nullable enable
        public string? Name { get; set; }
        public string? Owner { get; set; }
        public string? Client { get; set; }
        public int? Budget { get; set; }
        #nullable disable
    }
}
