using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FRF.DataAccess.EntityModels
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Client { get; set; }
        public int Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate{ get; set; }
        public IList<ProjectCategory> ProjectCategories { get; set; }
    }
}
