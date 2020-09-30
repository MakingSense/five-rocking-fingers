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
            CreateMap<ProjectCategory, ProjectCategoryDTO>()
                .ReverseMap();
            CreateMap<Category, CategoryDTO>()
                .ReverseMap();
        }
    }
}
