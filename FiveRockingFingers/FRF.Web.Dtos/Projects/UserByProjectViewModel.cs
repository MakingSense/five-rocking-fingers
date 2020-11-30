using System;

namespace FRF.Web.Dtos.Projects
{
    public class UserByProjectViewModel
    {
        public Guid? userId { get; set; }
        public string? fullname { get; set; }
        public string email { get; set; }
        public string? avatar { get; set; }
    }
}