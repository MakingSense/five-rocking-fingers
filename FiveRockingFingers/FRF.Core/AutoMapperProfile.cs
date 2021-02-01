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
            CreateMap<DataAccess.EntityModels.Artifact, AwsS3Artifact>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.ArtifactType, act => act.Ignore())
                .ForMember(dest => dest.Project, act => act.Ignore())
                .ForMember(dest => dest.WriteRequestsUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("writeRequestsUsed").Value))
                .ForMember(dest => dest.RetrieveRequestsUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("retrieveRequestsUsed").Value))
                .ForMember(dest => dest.StorageUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("storageUsed").Value))
                .ForMember(dest => dest.InfrequentAccessMultiplier, act =>
                    act.MapFrom(ar => ar.Settings.Element("infrequentAccessMultiplier").Value))
                .ForMember(dest => dest.StoragePrice, act =>
                    act.MapFrom(ar =>
                        Convert.ToDecimal(
                            ar.Settings.Element("product1").Element("pricingDimensions").Element("range0")
                                .Element("pricePerUnit").Value, CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.WriteRequestsPrice, act =>
                    act.MapFrom(ar =>
                        decimal.Parse(
                            ar.Settings.Element("product2").Element("pricingDimensions").Element("pricePerUnit").Value,
                            NumberStyles.Float)))
                .ForMember(dest => dest.RetrieveRequestsPrice, act =>
                    act.MapFrom(ar =>
                        decimal.Parse(
                            ar.Settings.Element("product3").Element("pricingDimensions").Element("pricePerUnit").Value,
                            NumberStyles.Float)));
            CreateMap<AwsS3Artifact, DataAccess.EntityModels.Artifact > ()
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