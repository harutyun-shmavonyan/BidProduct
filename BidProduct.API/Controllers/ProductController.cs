using BidProduct.API.ViewModels.Product;
using BidProduct.Common.Abstract;
using BidProduct.SL.Abstract;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BidProduct.API.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    public class ProductController : ControllerBase
    {
        private readonly IInternalMediator _mediator;
        private readonly IMapper _mapper;

        public ProductController(IInternalMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ProductReadViewModel>> GetAsync(long id)
        {
            var request = new GetProductQuery
            {
                Id = id
            };

            var response = await _mediator.SendAsync(request);
            var model = _mapper.Map<ProductReadViewModel>(response);

            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<ProductReadViewModel>> CreateAsync(ProductCreateViewModel createViewModel)
        {
            var command = _mapper.Map<CreateProductCommand>(createViewModel);
            var response = await _mediator.SendAsync(command);
            var model = _mapper.Map<ProductReadViewModel>(response);

            return CreatedAtAction(nameof(GetAsync), new { model.Id });
        }
    }
}
