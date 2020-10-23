using AutoMapper;
using FRF.Core.Models;
using FRF.Web.Dtos.Projects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectDto>()
                .ReverseMap();
            CreateMap<ProjectCategory, Projects.ProjectCategoryDTO>()
                .ReverseMap();
            CreateMap<Category, Projects.CategoryDTO>()
                .ReverseMap();
            CreateMap<Project, Categories.ProjectDTO>()
                .ReverseMap();
            CreateMap<ProjectCategory, Categories.ProjectCategoryDTO>()
                .ReverseMap();
            CreateMap<Category, Categories.CategoryDTO>()
                .ReverseMap();
        }
    }
}
