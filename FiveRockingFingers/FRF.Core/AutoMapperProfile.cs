using AutoMapper;
using FRF.Core.Models;
using FRF.DataAccess.EntityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace FRF.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DataAccess.EntityModels.Project, Models.Project>()
                .ReverseMap();
            CreateMap<DataAccess.EntityModels.Category, Models.Category>()
                .ReverseMap();
            CreateMap<DataAccess.EntityModels.ProjectCategory, Models.ProjectCategory>()
                .ReverseMap();
        }
    }
}
