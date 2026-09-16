using FunBooksAndVideos.Api.Validators;
using FunBooksAndVideos.Application.Dtos;

namespace FunBooksAndVideos.Tests.Api;

public class CreatePurchaseOrderRequestValidatorTests
{
    private readonly CreatePurchaseOrderRequestValidator _validator = new();

    [Fact]
    public void ValidRequest_Passes()
    {
        var request = new CreatePurchaseOrderRequest { CustomerId = 1, ProductIds = [1, 2, 2] };

        Assert.True(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void InvalidRequest_ReportsEveryProblem()
    {
        var result = _validator.Validate(new CreatePurchaseOrderRequest());

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreatePurchaseOrderRequest.CustomerId));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreatePurchaseOrderRequest.ProductIds));
    }

    [Fact]
    public void NonPositiveProductId_Fails()
    {
        var request = new CreatePurchaseOrderRequest { CustomerId = 1, ProductIds = [1, -3] };

        Assert.False(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void TooManyProducts_Fails()
    {
        var request = new CreatePurchaseOrderRequest
        {
            CustomerId = 1,
            ProductIds = Enumerable.Repeat(1, CreatePurchaseOrderRequestValidator.MaxProducts + 1).ToList()
        };

        Assert.False(_validator.Validate(request).IsValid);
    }
}
