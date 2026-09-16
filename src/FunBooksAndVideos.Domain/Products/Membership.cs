using FunBooksAndVideos.Domain.Memberships;

namespace FunBooksAndVideos.Domain.Products;

public class Membership : Product
{
    public Membership(string name, decimal price, MembershipType type) : base(name, price)
    {
        Type = type;
    }

    public MembershipType Type { get; set; }

    public override bool IsPhysical => false;
}
