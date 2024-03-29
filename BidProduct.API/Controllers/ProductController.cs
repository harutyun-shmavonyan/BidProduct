﻿using BidProduct.API.ViewModels.Product;
using BidProduct.Common.Abstract;
using BidProduct.DAL.Abstract.Filtering;
using BidProduct.DAL.Models;
using BidProduct.DAL.Models.Filters;
using BidProduct.SL.Abstract;
using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Models.CQRS.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BidProduct.API.Controllers
{
    [ApiController]
    [Route("[controller]s")]
    [Authorize]
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
        [ActionName(nameof(GetAsync))]
        public async Task<ActionResult<ProductReadViewModel>> GetAsync(long id)
        {
            await _mediator.SendAsync(new GetByFilterQuery<Product, long, GetProductQueryResponse>(new ProductFilter
            {
                OrderedProperties = {new OrderedProperty<Product>(OrderingForm.Ascending, p => p.Name)}
            }));

            var request = new GetProductQuery(id: id);

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

            var actionName = nameof(GetAsync);

            return CreatedAtAction(actionName, new { model.Id });
        }
    }
}
