﻿using BidProduct.SL.Abstract.CQRS;
using BidProduct.SL.Models.CQRS.Responses;

namespace BidProduct.SL.Models.CQRS.Commands
{
    public class CreateProductCommand : IInternalRequest<CreateProductCommandResponse>
    {
        public string Name { get; set; }

        public CreateProductCommand(string name)
        {
            Name = name;
        }
    }
}
