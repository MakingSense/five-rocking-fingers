using AutoMapper;
using FRF.Core.Models;
using FRF.Web.Dtos.Users;
using FRF.Web.Dtos.Projects;

namespace FRF.Web.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Core.Models.Project, ProjectDto>().ReverseMap();
            CreateMap<UserSignIn, SignInDTO>().ReverseMap();
            CreateMap<User, SignUpDTO>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<ProjectCategory, ProjectCategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
        }
    }
}
