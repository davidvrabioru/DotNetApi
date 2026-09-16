using FunBooksAndVideos.Domain.Common;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Domain.Shipping;

public class ShippingSlip : AuditableEntity
{
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public int CustomerId { get; set; }
    public List<ShippingSlipLine> Lines { get; set; } = [];
}
