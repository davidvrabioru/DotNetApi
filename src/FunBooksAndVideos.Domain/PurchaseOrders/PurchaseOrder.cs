using FunBooksAndVideos.Domain.Common;

namespace FunBooksAndVideos.Domain.PurchaseOrders;

public class PurchaseOrder : AuditableEntity
{
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public PurchaseOrderStatus Status { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public List<PurchaseOrderLine> Lines { get; set; } = [];
}
