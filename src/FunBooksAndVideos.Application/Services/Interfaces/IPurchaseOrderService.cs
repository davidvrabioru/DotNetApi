using FunBooksAndVideos.Application.Dtos;

namespace FunBooksAndVideos.Application.Services.Interfaces;

public interface IPurchaseOrderService
{
    Task<PurchaseOrderDto?> PlaceOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ShippingSlipDto?> GetShippingSlipAsync(int purchaseOrderId, CancellationToken cancellationToken = default);
}
