using System.Collections.Generic;

namespace FRF.Web.Dtos.Modules
{
    public class ModuleDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SuggestedCost { get; set; }
        public IList<CategoryModuleDTO> CategoryModules { get; set; }
    }
}