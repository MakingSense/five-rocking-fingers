using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core.Models
{
    public class UsersByProject
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public Guid UserId { get; set; }
    }
}