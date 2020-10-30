using AutoMapper;
using FRF.Core.Models;
using FRF.Web.Dtos.Artifacts;
using FRF.Web.Dtos.Projects;
using FRF.Web.Dtos.Users;

namespace FRF.Web.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserSignIn, SignInDTO>().ReverseMap();
            CreateMap<User, SignUpDTO>().ReverseMap();
            CreateMap<Project, ProjectDto>()
                .ReverseMap();
            CreateMap<ProjectCategory, Projects.ProjectCategoryDTO>()
                .ReverseMap();
            CreateMap<Category, Projects.CategoryDTO>()
                .ReverseMap();
            CreateMap<Project, Categories.ProjectDTO>()
                .ReverseMap();
            CreateMap<ProjectCategory, Categories.ProjectCategoryDTO>()
                .ReverseMap();
            CreateMap<Category, Categories.CategoryDTO>()
                .ReverseMap();
            CreateMap<Artifact, ArtifactDTO>()
                .ReverseMap()
                .ForMember(dest => dest.ArtifactTypeId, opt => opt.MapFrom(src => src.ArtifactType.Id))
                .ForMember(dest => dest.ProjectId, opt => opt.MapFrom(src => src.ProjectId));
            CreateMap<ArtifactType, ArtifactTypeDTO>()
                .ReverseMap();
        }
    }
}