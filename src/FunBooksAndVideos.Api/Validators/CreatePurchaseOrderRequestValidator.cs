using FluentValidation;
using FunBooksAndVideos.Application.Dtos;

namespace FunBooksAndVideos.Api.Validators;

public class CreatePurchaseOrderRequestValidator : AbstractValidator<CreatePurchaseOrderRequest>
{
    public const int MaxProducts = 100;

    public CreatePurchaseOrderRequestValidator()
    {
        RuleFor(request => request.CustomerId)
            .GreaterThan(0);

        RuleFor(request => request.ProductIds)
            .NotEmpty()
            .WithMessage("A purchase order must contain at least one product.")
            .Must(productIds => productIds.Count <= MaxProducts)
            .WithMessage($"A purchase order can contain at most {MaxProducts} products.");

        RuleForEach(request => request.ProductIds)
            .GreaterThan(0);
    }
}
