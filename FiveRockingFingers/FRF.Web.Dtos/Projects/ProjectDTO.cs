using FRF.Web.Dtos.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper.Configuration.Conventions;

namespace FRF.Web.Dtos
{
    public class ProjectDTO
    {
        [Required]
        public int Id { get; set; }
        [Required(ErrorMessage = "Project name is required")]
        public string Name { get; set; }
        public string Owner { get; set; }
        public string? Client { get; set; }
        public int? Budget { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<ProjectCategoryDTO> ProjectCategories { get; set; }
        public IList<UserProfile> UsersProfile { get; set; }
    }
}