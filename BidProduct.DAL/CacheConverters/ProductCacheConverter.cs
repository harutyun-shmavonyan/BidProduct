using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract.Cache;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.DAL.CacheConverters
{
    public class ProductCacheConverter : 
        ICacheKeyConverter<GetProductQuery, string>, 
        ICacheValueConverter<GetProductQueryResponse, object>
    {
        private readonly IMapper _mapper;

        public ProductCacheConverter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public string ToKey(GetProductQuery input) => input.Id.ToString();

        public object ConvertToInternalValue(GetProductQueryResponse value) => _mapper.Map<GetProductQueryResponse>(value);
        public GetProductQueryResponse ConvertToExternalValue(object value) => _mapper.Map<GetProductQueryResponse>(value);
    }
}
