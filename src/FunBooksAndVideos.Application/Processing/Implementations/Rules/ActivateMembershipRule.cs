using FunBooksAndVideos.Domain.Memberships;
using FunBooksAndVideos.Domain.Products;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.Processing.Implementations.Rules;

public class ActivateMembershipRule : PurchaseOrderRule
{
    protected override bool AppliesTo(PurchaseOrderProcessingContext context)
    {
        return GetMemberships(context.Order).Any();
    }

    protected override Task ExecuteAsync(PurchaseOrderProcessingContext context, CancellationToken cancellationToken)
    {
        foreach (var membership in GetMemberships(context.Order))
        {
            context.Customer.Membership = Combine(context.Customer.Membership, membership.Type);
        }

        return Task.CompletedTask;
    }

    private static IEnumerable<Membership> GetMemberships(PurchaseOrder order)
    {
        return order.Lines.Select(line => line.Product).OfType<Membership>();
    }

    private static MembershipType Combine(MembershipType current, MembershipType purchased)
    {
        if (current == MembershipType.Premium || purchased == MembershipType.Premium)
        {
            return MembershipType.Premium;
        }

        if ((current == MembershipType.BookClub && purchased == MembershipType.VideoClub) ||
            (current == MembershipType.VideoClub && purchased == MembershipType.BookClub))
        {
            return MembershipType.Premium;
        }

        return purchased;
    }
}
