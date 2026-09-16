namespace FunBooksAndVideos.Application.Dtos;

public class CreatePurchaseOrderRequest
{
    public int CustomerId { get; set; }
    public List<int> ProductIds { get; set; } = [];
}
