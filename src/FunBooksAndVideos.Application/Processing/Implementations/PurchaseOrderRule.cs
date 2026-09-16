using FunBooksAndVideos.Application.Processing;
using FunBooksAndVideos.Application.Processing.Interfaces;

namespace FunBooksAndVideos.Application.Processing.Implementations;

public abstract class PurchaseOrderRule : IPurchaseOrderRule
{
    public async Task ApplyAsync(PurchaseOrderProcessingContext context, CancellationToken cancellationToken = default)
    {
        if (AppliesTo(context))
        {
            await ExecuteAsync(context, cancellationToken);
        }
    }

    protected abstract bool AppliesTo(PurchaseOrderProcessingContext context);

    protected abstract Task ExecuteAsync(PurchaseOrderProcessingContext context, CancellationToken cancellationToken);
}
