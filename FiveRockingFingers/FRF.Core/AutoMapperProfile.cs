using System;
using System.Globalization;
using AutoMapper;
using FRF.Core.Models;
using FRF.Core.Models.AwsArtifacts;

namespace FRF.Core
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<DataAccess.EntityModels.Project, Models.Project>()
                .ReverseMap()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.UsersByProject, act => act.Ignore())
                .ForMember(dest => dest.ProjectCategories, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.Category, Models.Category>()
                .ReverseMap()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.ProjectCategories, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.ProjectCategory, Models.ProjectCategory>()
                .ReverseMap();
            CreateMap<DataAccess.EntityModels.Artifact, Models.Artifact>()
                .ReverseMap()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.ArtifactType, act => act.Ignore())
                .ForMember(dest => dest.Project, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.Artifact, AwsS3>()
                .ForMember(dest => dest.WriteRequestsUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("writeRequestsUsed").Value))
                .ForMember(dest => dest.RetrieveRequestsUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("retrieveRequestsUsed").Value))
                .ForMember(dest => dest.StorageUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("storageUsed").Value))
                .ForMember(dest => dest.InfrequentAccessMultiplier, act =>
                    act.MapFrom(ar => ar.Settings.Element("infrequentAccessMultiplier").Value));
            CreateMap<DataAccess.EntityModels.Artifact, Models.CustomArtifact>()
                .ReverseMap()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.ArtifactType, act => act.Ignore())
                .ForMember(dest => dest.Project, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.Artifact, Models.AwsArtifacts.AwsEc2>();
            CreateMap<DataAccess.EntityModels.ArtifactType, Models.ArtifactType>()
                .ReverseMap();
            CreateMap<DataAccess.EntityModels.UsersByProject, Models.UsersProfile>()
                .ReverseMap()
                .ForMember(dest => dest.Id, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.ArtifactsRelation, ArtifactsRelation>()
                .ReverseMap();
            CreateMap<DataAccess.EntityModels.Provider, Provider>();
        }
    }
}
