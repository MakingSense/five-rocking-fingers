using AutoMapper;
using FRF.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Web.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Core.Models.Project, ProjectDto>()
                .ReverseMap();
            CreateMap<Core.Models.Project, ProjectCategoryDTO>()
                .ReverseMap();
        }
    }
}
