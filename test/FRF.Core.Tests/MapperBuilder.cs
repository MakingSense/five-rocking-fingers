using AutoMapper;

namespace FRF.Core.Tests
{
    public static class MapperBuilder
    {
        public static IMapper Build()
        {
            var mapperConfiguration = new MapperConfiguration(mc =>
            {
                mc.AddProfile<AutoMapperProfile>();
            });

            var mapper = new Mapper(mapperConfiguration);
            return mapper;
        }
    }
}
