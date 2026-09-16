namespace FunBooksAndVideos.Application.Dtos;

public class PurchaseOrderLineDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
