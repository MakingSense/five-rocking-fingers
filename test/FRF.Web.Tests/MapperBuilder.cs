using AutoMapper;

namespace FRF.Web.Tests
{
    public static class MapperBuilder
    {
        public static IMapper Build()
        {
            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile<Core.AutoMapperProfile>();
                mc.AddProfile<Dtos.AutoMapperProfile>();
            });

            var mapper = new Mapper(mapperConfiguration);
            return mapper;
        }
    }
}
