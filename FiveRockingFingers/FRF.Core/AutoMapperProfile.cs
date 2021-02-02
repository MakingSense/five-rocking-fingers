using AutoMapper;
using FRF.Core.Models;

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
            CreateMap<DataAccess.EntityModels.Artifact, Models.CustomArtifact>()
                .ReverseMap()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.ArtifactType, act => act.Ignore())
                .ForMember(dest => dest.Project, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.ArtifactType, Models.ArtifactType>()
                .ReverseMap();
            CreateMap<DataAccess.EntityModels.UsersByProject, Models.UsersProfile>()
                .ReverseMap()
                .ForMember(dest => dest.Id, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.ArtifactsRelation, ArtifactsRelation>()
                .ReverseMap();
        }
    }
}