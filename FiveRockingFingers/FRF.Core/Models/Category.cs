using System.Collections.Generic;

namespace FRF.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        #nullable enable
        public string? Description { get; set; }
#nullable disable
        public IList<ProjectCategory> ProjectCategories { get; set; }
        public IList<CategoryModule> CategoryModules { get; set; }
    }
}
