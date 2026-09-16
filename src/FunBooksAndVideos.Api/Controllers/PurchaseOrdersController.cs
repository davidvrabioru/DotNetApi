using FluentValidation;
using FunBooksAndVideos.Application.Dtos;
using FunBooksAndVideos.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FunBooksAndVideos.Api.Controllers;

[ApiController]
[Route("api/purchase-orders")]
public class PurchaseOrdersController : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService;
    private readonly IValidator<CreatePurchaseOrderRequest> _validator;

    public PurchaseOrdersController(
        IPurchaseOrderService purchaseOrderService,
        IValidator<CreatePurchaseOrderRequest> validator)
    {
        _purchaseOrderService = purchaseOrderService;
        _validator = validator;
    }

    [HttpPost]
    public async Task<ActionResult<PurchaseOrderDto>> Create(
        CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
        }

        var order = await _purchaseOrderService.PlaceOrderAsync(request, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PurchaseOrderDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await _purchaseOrderService.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpGet("{id:int}/shipping-slip")]
    public async Task<ActionResult<ShippingSlipDto>> GetShippingSlip(int id, CancellationToken cancellationToken)
    {
        var slip = await _purchaseOrderService.GetShippingSlipAsync(id, cancellationToken);
        if (slip is null)
        {
            return NotFound();
        }

        return Ok(slip);
    }
}
