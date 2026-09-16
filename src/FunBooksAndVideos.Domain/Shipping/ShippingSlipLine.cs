using FunBooksAndVideos.Domain.Common;

namespace FunBooksAndVideos.Domain.Shipping;

public class ShippingSlipLine : Entity
{
    public int ShippingSlipId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
}
