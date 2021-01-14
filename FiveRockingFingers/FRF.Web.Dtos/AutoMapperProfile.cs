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
            CreateMap<Project, ProjectDTO>()
                .ForMember(dest => dest.Users, opt =>
                    opt.MapFrom(src => src.UsersByProject))
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
            CreateMap<Categories.CategoryUpsertDTO, Category>();
            CreateMap<Artifact, ArtifactDTO>()
                .ForMember(dest => dest.Settings, opt => opt.MapFrom<XMLHelper>());
            CreateMap<ArtifactUpsertDTO, Artifact>();
            CreateMap<ArtifactType, ArtifactTypeDTO>()
                .ReverseMap();
            CreateMap<UsersProfile, UserProfileDTO>()
                .ReverseMap();
            CreateMap<ProjectUpsertDTO, Project>()
                .ForMember(dest => dest.UsersByProject, opt => opt.MapFrom(src => src.Users));
            CreateMap<UserProfileUpsertDTO, UsersProfile>();
            CreateMap<ArtifactsRelationDTO, ArtifactsRelation>()
                .ReverseMap();
            CreateMap<ArtifactsRelationUpsertDTO, ArtifactsRelation>();
        }
    }
}