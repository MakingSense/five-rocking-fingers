using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FRF.Web.Dtos.ProjectModules;

namespace FRF.Web.Dtos.Modules
{
    public class ModuleUpsertDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int SuggestedCost { get; set; }
        public IList<CategoryModuleUpsertDTO> CategoryModules { get; set; }
    }
}