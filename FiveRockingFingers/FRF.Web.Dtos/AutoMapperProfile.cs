using AutoMapper;
using FRF.Core.Models;
using FRF.Web.Dtos.Users;

namespace FRF.Web.Dtos
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Core.Models.Project, ProjectDto>()
                .ReverseMap();
            CreateMap<UserSignIn, SignInDTO>().ReverseMap();
            CreateMap<User, SignUpDTO>().ReverseMap();
        }
    }
}