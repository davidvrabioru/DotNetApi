using FunBooksAndVideos.Domain.Common;
using FunBooksAndVideos.Domain.Products;

namespace FunBooksAndVideos.Domain.PurchaseOrders;

public class PurchaseOrderLine : Entity
{
    public int PurchaseOrderId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
