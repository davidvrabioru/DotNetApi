using FunBooksAndVideos.Application.Processing;

namespace FunBooksAndVideos.Application.Processing.Interfaces;

public interface IPurchaseOrderRule
{
    Task ApplyAsync(PurchaseOrderProcessingContext context, CancellationToken cancellationToken = default);
}
