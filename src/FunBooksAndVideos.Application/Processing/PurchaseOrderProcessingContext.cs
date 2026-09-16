using FunBooksAndVideos.Domain.Customers;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.Processing;

public class PurchaseOrderProcessingContext
{
    public PurchaseOrder Order { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
}
