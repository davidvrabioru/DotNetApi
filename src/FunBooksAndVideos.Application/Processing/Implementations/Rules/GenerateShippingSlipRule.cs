using FunBooksAndVideos.Domain.PurchaseOrders;
using FunBooksAndVideos.Domain.Shipping;
using FunBooksAndVideos.Infrastructure.Persistence;

namespace FunBooksAndVideos.Application.Processing.Implementations.Rules;

public class GenerateShippingSlipRule : PurchaseOrderRule
{
    private readonly AppDbContext _dbContext;

    public GenerateShippingSlipRule(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override bool AppliesTo(PurchaseOrderProcessingContext context)
    {
        return GetPhysicalLines(context.Order).Any();
    }

    protected override async Task ExecuteAsync(PurchaseOrderProcessingContext context, CancellationToken cancellationToken)
    {
        var slip = new ShippingSlip
        {
            PurchaseOrder = context.Order,
            CustomerId = context.Order.CustomerId,
            Lines = GetPhysicalLines(context.Order)
                .Select(line => new ShippingSlipLine
                {
                    ProductId = line.ProductId,
                    ProductName = line.ProductName
                })
                .ToList()
        };

        await _dbContext.ShippingSlips.AddAsync(slip, cancellationToken);
    }

    private static IEnumerable<PurchaseOrderLine> GetPhysicalLines(PurchaseOrder order)
    {
        return order.Lines.Where(line => line.Product.IsPhysical);
    }
}
