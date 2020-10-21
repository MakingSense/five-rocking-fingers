﻿using AutoMapper;
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
                .ReverseMap().ForMember(dest => dest.Id, act => act.Ignore()).ForMember(dest => dest.ProjectCategories, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.Category, Models.Category>()
                .ReverseMap().ForMember(dest => dest.Id, act => act.Ignore()).ForMember(dest => dest.ProjectCategories, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.ProjectCategory, Models.ProjectCategory>()
                .ReverseMap();
        }
    }
}
