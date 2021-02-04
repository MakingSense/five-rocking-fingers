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
            CreateMap<DataAccess.EntityModels.Artifact, Models.AwsArtifacts.AmazonEc2Artifact>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.ArtifactType, act => act.Ignore())
                .ForMember(dest => dest.Project, act => act.Ignore())
                .ForMember(dest => dest.PricePerUnit0, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("range0").Element("pricePerUnit").Value)))
                .ForMember(dest => dest.PricePerUnit1, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("range1").Element("pricePerUnit").Value)))
                .ForMember(dest => dest.PurchaseOption, act => act.MapFrom(ar => ar.Settings.Element("purchaseOption").Value))
                .ForMember(dest => dest.PurchaseOption, act => act.MapFrom(ar => ar.Settings.Element("leaseContractLength").Value));

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