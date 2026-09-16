using FunBooksAndVideos.Domain.Memberships;

namespace FunBooksAndVideos.Application.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsPhysical { get; set; }
    public string? Author { get; set; }
    public int? DurationMinutes { get; set; }
    public MembershipType? MembershipType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
