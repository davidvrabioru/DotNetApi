using FunBooksAndVideos.Domain.Common;
using FunBooksAndVideos.Domain.Memberships;

namespace FunBooksAndVideos.Domain.Customers;

public class Customer : AuditableEntity
{
    public Customer(string name, string email)
    {
        Name = name;
        Email = email;
    }

    public string Name { get; set; }
    public string Email { get; set; }
    public MembershipType Membership { get; set; }
}
