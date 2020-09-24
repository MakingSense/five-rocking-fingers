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
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public virtual ProjectCategory ProjectCategory { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime StartDate { get; set; }
    }

    public class ProjectCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
