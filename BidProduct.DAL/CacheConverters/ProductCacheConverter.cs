using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.DAL.CacheConverters
{
    public class ProductCacheConverter : 
        ICacheKeyConverter<GetProductQuery, long>, 
        ICacheValueConverter<GetProductQueryResponse, GetProductQueryResponse>
    {
        private readonly IMapper _mapper;

        public ProductCacheConverter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public long ToKey(GetProductQuery input) => input.Id;

        public GetProductQueryResponse ConvertToInternalValue(GetProductQueryResponse value) => _mapper.Map<GetProductQueryResponse>(value);

        public GetProductQueryResponse ConvertToExternalValue(GetProductQueryResponse value) => _mapper.Map<GetProductQueryResponse>(value);
    }
}
