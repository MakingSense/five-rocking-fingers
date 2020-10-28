using AutoMapper;
using FRF.Core.Models;
using FRF.Web.Dtos.Artifacts;
using FRF.Web.Dtos.Projects;

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
            CreateMap<Artifact, ArtifactDTO>()
                .ReverseMap()
                .ForMember(dest=> dest.ArtifactTypeId, opt => opt.MapFrom(src => src.ArtifactType.Id))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId));
            CreateMap<ArtifactType, ArtifactTypeDTO>()
                .ReverseMap();
        }
    }
}
