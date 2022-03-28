using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.SL.CQRS.QueryHandlers
{
    public class GetProductQueryHandler : IInternalRequestHandler<GetProductQuery, GetProductQueryResponse>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _repository;

        public GetProductQueryHandler(IProductRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GetProductQueryResponse> HandleAsync(GetProductQuery request, CancellationToken ct = default)
        {
            var product = await _repository.GetByIdAsync(request.Id);

            return _mapper.Map<GetProductQueryResponse>(product);
        }
    }
}
