using FunBooksAndVideos.Application.Processing;
using FunBooksAndVideos.Application.Processing.Interfaces;
using FunBooksAndVideos.Domain.PurchaseOrders;

namespace FunBooksAndVideos.Application.Processing.Implementations;

public class PurchaseOrderProcessor : IPurchaseOrderProcessor
{
    private readonly IEnumerable<IPurchaseOrderRule> _rules;
    private readonly TimeProvider _timeProvider;

    public PurchaseOrderProcessor(IEnumerable<IPurchaseOrderRule> rules, TimeProvider timeProvider)
    {
        _rules = rules;
        _timeProvider = timeProvider;
    }

    public async Task ProcessAsync(PurchaseOrderProcessingContext context, CancellationToken cancellationToken = default)
    {
        foreach (var rule in _rules)
        {
            await rule.ApplyAsync(context, cancellationToken);
        }

        context.Order.Status = PurchaseOrderStatus.Processed;
        context.Order.ProcessedAt = _timeProvider.GetUtcNow();
    }
}
