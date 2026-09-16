using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.Dtos;

public class PurchaseOrderDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public List<PurchaseOrderLineDto> Lines { get; set; } = [];
}
