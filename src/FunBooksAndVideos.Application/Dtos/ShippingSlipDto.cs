namespace FunBooksAndVideos.Application.Dtos;

public class ShippingSlipDto
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<ShippingSlipLineDto> Lines { get; set; } = [];
}
