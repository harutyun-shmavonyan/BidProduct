namespace BidProduct.Common.Abstract
{
    public interface IMapper
    {
        TDestination Map<TDestination>(object source);
        ICollection<TDestination> MapMany<TSource, TDestination>(ICollection<TSource> sources);

        TDestination Map<TDestination, TOptions>(object source, TOptions options);

        ICollection<TDestination> MapMany<TSource, TDestination, TOptions>(ICollection<TSource> sources,
            TOptions options);
    }
}
