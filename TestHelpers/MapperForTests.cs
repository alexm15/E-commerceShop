using AutoMapper;
using E_commercePIM.Mapping;

namespace TestHelpers
{
    public class MapperForTests
    {
        public MapperForTests()
        {
            var config = new MapperConfiguration(opts => { opts.AddProfile(new ViewModelsProfile()); });
            Mapper = config.CreateMapper();
        }

        public IMapper Mapper { get; }
    }
}