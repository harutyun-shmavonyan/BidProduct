using BidProduct.SL.Models.CQRS.Queries;
using BidProduct.SL.Validators.Abstract;
using FluentValidation;

namespace BidProduct.SL.Validators
{
    public class GetProductQueryValidator : SimpleValidatorBase<GetProductQuery>
    {
        public GetProductQueryValidator()
        {
            RuleFor(p => p.Id).GreaterThan(0);
        }
    }
}
