using FunBooksAndVideos.Application.Processing;
using FunBooksAndVideos.Application.Processing.Implementations;
using FunBooksAndVideos.Application.Processing.Interfaces;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Tests.Application;

public class PurchaseOrderProcessorTests
{
    [Fact]
    public async Task ProcessAsync_RunsEveryRuleInOrder_ThenMarksOrderProcessed()
    {
        var calls = new List<string>();
        var rules = new List<IPurchaseOrderRule>
        {
            new RecordingRule("first", calls),
            new RecordingRule("second", calls)
        };
        var processedAt = new DateTimeOffset(2026, 9, 16, 12, 0, 0, TimeSpan.Zero);
        var processor = new PurchaseOrderProcessor(rules, new FixedTimeProvider(processedAt));
        var context = TestData.Context(TestData.Customer(), TestData.Book());

        await processor.ProcessAsync(context);

        Assert.Equal(["first", "second"], calls);
        Assert.Equal(PurchaseOrderStatus.Processed, context.Order.Status);
        Assert.Equal(processedAt, context.Order.ProcessedAt);
    }

    private class RecordingRule : IPurchaseOrderRule
    {
        private readonly string _name;
        private readonly List<string> _calls;

        public RecordingRule(string name, List<string> calls)
        {
            _name = name;
            _calls = calls;
        }

        public Task ApplyAsync(PurchaseOrderProcessingContext context, CancellationToken cancellationToken = default)
        {
            _calls.Add(_name);
            return Task.CompletedTask;
        }
    }

    private class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedTimeProvider(DateTimeOffset now)
        {
            _now = now;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _now;
        }
    }
}
