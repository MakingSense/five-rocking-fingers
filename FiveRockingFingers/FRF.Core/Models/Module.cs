﻿using System.Collections.Generic;

namespace FRF.Core.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SuggestedCost { get; set; }
        public IList<ProjectModule> ProjectModules { get; set; }
    }
}