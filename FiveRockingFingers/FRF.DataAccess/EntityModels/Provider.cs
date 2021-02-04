using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FRF.DataAccess.EntityModels
{
    public class Provider
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ArtifactType> ArtifactType { get; set; }
    }
}
