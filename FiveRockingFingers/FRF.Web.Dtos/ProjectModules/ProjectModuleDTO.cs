using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos.ProjectModules
{
    public class ProjectModuleDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ModuleId { get; set; }
        public string Alias { get; set; }
        public int Cost { get; set; }
    }
}
