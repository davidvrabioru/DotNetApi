using FunBooksAndVideos.Application.Processing.Implementations.Rules;

namespace FunBooksAndVideos.Tests.Application;

public class GenerateShippingSlipRuleTests
{
    [Fact]
    public async Task OrderWithPhysicalProduct_GetsSlipWithOnlyPhysicalLines()
    {
        await using var dbContext = TestData.DbContext();
        var rule = new GenerateShippingSlipRule(dbContext);
        var context = TestData.Context(TestData.Customer(), TestData.Video(), TestData.Book());

        await rule.ApplyAsync(context);

        var slip = Assert.Single(dbContext.ShippingSlips.Local);
        Assert.Same(context.Order, slip.PurchaseOrder);
        Assert.Equal("The Girl on the Train", Assert.Single(slip.Lines).ProductName);
    }

    [Fact]
    public async Task DigitalOnlyOrder_GetsNoSlip()
    {
        await using var dbContext = TestData.DbContext();
        var rule = new GenerateShippingSlipRule(dbContext);
        var context = TestData.Context(TestData.Customer(), TestData.Video(), TestData.Membership());

        await rule.ApplyAsync(context);

        Assert.Empty(dbContext.ShippingSlips.Local);
    }
}
