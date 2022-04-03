using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract;
using BidProduct.DAL.Models;
using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.SL.CQRS.QueryHandlers;

public class CreateProductCommandHandler : IInternalRequestHandler<CreateProductCommand, CreateProductCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IProductRepository repository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CreateProductCommandResponse> HandleAsync(CreateProductCommand command, CancellationToken ct = default)
    {
        var product = _mapper.Map<Product>(command);

        product = _repository.Add(product);
        await _unitOfWork.SaveAsync();

        return _mapper.Map<CreateProductCommandResponse>(product);
    }
}