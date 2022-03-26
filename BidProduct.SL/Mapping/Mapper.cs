using AutoMapper;
using IMapper = BidProduct.Common.Abstract.IMapper;

namespace BidProduct.SL.Mapping
{
    public class Mapper : IMapper
    {
        private readonly AutoMapper.IMapper _mapper;

        public Mapper(AutoMapper.IMapper mapper)
        {
            _mapper = mapper;
            //_mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        public TDestination Map<TDestination>(object source) =>
            _mapper.Map<TDestination>(source);

        public ICollection<TDestination> MapMany<TSource, TDestination>(ICollection<TSource> sources) =>
            sources.Select(s => _mapper.Map<TDestination>(s)).ToList();

        public TDestination Map<TDestination, TOptions>(object source, TOptions options) =>
            _mapper.Map(source, options as Action<IMappingOperationOptions<object, TDestination>>);

        public ICollection<TDestination> MapMany<TSource, TDestination, TOptions>(ICollection<TSource> sources,
            TOptions options) =>
            sources.Select(s => _mapper.Map(s, options as Action<IMappingOperationOptions<object, TDestination>>))
                .ToList();
    }
}
