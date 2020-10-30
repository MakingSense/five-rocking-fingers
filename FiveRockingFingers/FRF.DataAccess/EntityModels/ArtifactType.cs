using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FRF.DataAccess.EntityModels
{
    public class ArtifactType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Artifact> Artifacs { get; set; }
    }
}
