using BidProduct.SL.Models.CQRS.Commands;
using BidProduct.SL.Validators.Abstract;
using FluentValidation;

namespace BidProduct.SL.Validators;

public class CreateProductCommandValidator : SimpleValidatorBase<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Name).MinimumLength(3).MaximumLength(128);
    }
}