﻿using System.Collections.Generic;

namespace FRF.Core.Models
{
    public class ArtifactType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Artifact> Artifacs { get; set; }
    }
}