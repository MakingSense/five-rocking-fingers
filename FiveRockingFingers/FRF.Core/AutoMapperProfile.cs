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
                .ForMember(dest => dest.Project, act => act.Ignore());
            CreateMap<DataAccess.EntityModels.Artifact, AwsS3>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.Project, act => act.Ignore())
                .ForMember(dest => dest.WriteRequestsUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("writeRequestsUsed").Value))
                .ForMember(dest => dest.RetrieveRequestsUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("retrieveRequestsUsed").Value))
                .ForMember(dest => dest.StorageUsed, act =>
                    act.MapFrom(ar => ar.Settings.Element("storageUsed").Value))
                .ForMember(dest => dest.InfrequentAccessMultiplier, act =>
                    act.MapFrom(ar => ar.Settings.Element("infrequentAccessMultiplier").Value));
            CreateMap<AwsS3, DataAccess.EntityModels.Artifact > ()
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
            CreateMap<DataAccess.EntityModels.Artifact, Models.AwsArtifacts.AwsEc2>()
                .ForMember(dest => dest.Id, act => act.Ignore())
                .ForMember(dest => dest.ArtifactType, act => act.Ignore())
                .ForMember(dest => dest.Project, act => act.Ignore())
                //Compute instance properties
                .ForMember(dest => dest.HoursUsedPerMonth, act => act.MapFrom(ar => Int32.Parse(ar.Settings.Element("product0").Element("HoursUsedPerMonth").Value)))
                .ForMember(dest => dest.InstancePricePerUnit0, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("product0").Element("pricingDimensions").Element("range0").Element("pricePerUnit").Value)))
                .ForMember(dest => dest.InstancePricePerUnit1, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("product0").Element("pricingDimensions").Element("range1").Element("pricePerUnit").Value)))
                .ForMember(dest => dest.PurchaseOption, act => act.MapFrom(ar => ar.Settings.Element("product0").Element("purchaseOption").Value))
                .ForMember(dest => dest.LeaseContractLength, act => act.MapFrom(ar => ar.Settings.Element("product0").Element("leaseContractLength").Value))
                //EBS Storage properties
                .ForMember(dest => dest.VolumenApiName, act => act.MapFrom(ar => ar.Settings.Element("product1").Element("volumeApiName").Value))
                .ForMember(dest => dest.NumberOfGbStorageInEbs, act => act.MapFrom(ar => Int32.Parse(ar.Settings.Element("product1").Element("numberOfGbStorageInEbs").Value)))
                .ForMember(dest => dest.EbsPricePerUnit, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("product1").Element("pricingDimensions").Element("range0").Element("pricePerUnit").Value)))
                //EBS Snapshots properties
                .ForMember(dest => dest.NumberOfSnapshotsPerMonth, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("product2").Element("numberOfSnapshotsPerMonth").Value)))
                .ForMember(dest => dest.NumberOfGbChangedPerSnapshot, act => act.MapFrom(ar => Int32.Parse(ar.Settings.Element("product2").Element("numberOfGbChangedPerSnapshot").Value)))
                .ForMember(dest => dest.SnapshotPricePerUnit, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("product2").Element("pricingDimensions").Element("range0").Element("pricePerUnit").Value)))
                //EBS IOPS properties
                .ForMember(dest => dest.NumberOfIops, act => act.MapFrom(ar => Int32.Parse(ar.Settings.Element("product3").Element("numberOfIopsPerMonth").Value)))
                .ForMember(dest => dest.IopsPricePerUnit, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("product3").Element("pricingDimensions").Element("range0").Element("pricePerUnit").Value)))
                //EBS throughput properties
                .ForMember(dest => dest.NumberOfMbpsThroughput, act => act.MapFrom(ar => Int32.Parse(ar.Settings.Element("product4").Element("numberOfMbpsThroughput").Value)))
                .ForMember(dest => dest.SnapshotPricePerUnit, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("product4").Element("pricingDimensions").Element("range0").Element("pricePerUnit").Value)))
                //Data Transfer properties
                .ForMember(dest => dest.NumberOfGbTransferIntraRegion, act => act.MapFrom(ar => Int32.Parse(ar.Settings.Element("product5").Element("numberOfGbTransferIntraRegion").Value)))
                .ForMember(dest => dest.IntraTegionDataTransferPricePerUnit, act => act.MapFrom(ar => decimal.Parse(ar.Settings.Element("product5").Element("pricingDimensions").Element("range0").Element("pricePerUnit").Value))); ;

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