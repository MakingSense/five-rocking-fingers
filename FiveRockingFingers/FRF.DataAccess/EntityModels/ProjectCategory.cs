using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.DataAccess.EntityModels
{
    public class ProjectCategory
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int CategoryID { get; set; }
        public Category Category { get; set; }
    }
}
