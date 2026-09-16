using FunBooksAndVideos.Domain.Memberships;

namespace FunBooksAndVideos.Application.Dtos;

public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public MembershipType Membership { get; set; }
}
