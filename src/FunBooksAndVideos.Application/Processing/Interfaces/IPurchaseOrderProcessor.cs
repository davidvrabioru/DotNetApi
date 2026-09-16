using FunBooksAndVideos.Application.Processing;

namespace FunBooksAndVideos.Application.Processing.Interfaces;

public interface IPurchaseOrderProcessor
{
    Task ProcessAsync(PurchaseOrderProcessingContext context, CancellationToken cancellationToken = default);
}
