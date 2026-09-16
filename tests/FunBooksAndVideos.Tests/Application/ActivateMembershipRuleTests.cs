using FunBooksAndVideos.Application.Processing.Implementations.Rules;
using FunBooksAndVideos.Domain.Memberships;

namespace FunBooksAndVideos.Tests.Application;

public class ActivateMembershipRuleTests
{
    private readonly ActivateMembershipRule _rule = new();

    [Theory]
    [InlineData(MembershipType.None, MembershipType.BookClub, MembershipType.BookClub)]
    [InlineData(MembershipType.None, MembershipType.Premium, MembershipType.Premium)]
    [InlineData(MembershipType.BookClub, MembershipType.BookClub, MembershipType.BookClub)]
    [InlineData(MembershipType.BookClub, MembershipType.VideoClub, MembershipType.Premium)]
    [InlineData(MembershipType.VideoClub, MembershipType.BookClub, MembershipType.Premium)]
    [InlineData(MembershipType.Premium, MembershipType.BookClub, MembershipType.Premium)]
    public async Task PurchasedMembership_IsActivatedOnCustomer(
        MembershipType current, MembershipType purchased, MembershipType expected)
    {
        var context = TestData.Context(TestData.Customer(current), TestData.Book(), TestData.Membership(purchased));

        await _rule.ApplyAsync(context);

        Assert.Equal(expected, context.Customer.Membership);
    }

    [Fact]
    public async Task OrderWithoutMembership_LeavesCustomerUnchanged()
    {
        var context = TestData.Context(TestData.Customer(), TestData.Book(), TestData.Video());

        await _rule.ApplyAsync(context);

        Assert.Equal(MembershipType.None, context.Customer.Membership);
    }
}
