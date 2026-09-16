using FunBooksAndVideos.Application.Dtos;
using FunBooksAndVideos.Application.Processing;
using FunBooksAndVideos.Application.Processing.Interfaces;
using FunBooksAndVideos.Application.Services.Interfaces;
using FunBooksAndVideos.Domain.PurchaseOrders;
using FunBooksAndVideos.Domain.Shipping;
using FunBooksAndVideos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FunBooksAndVideos.Application.Services.Implementations;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly AppDbContext _dbContext;
    private readonly IPurchaseOrderProcessor _processor;

    public PurchaseOrderService(AppDbContext dbContext, IPurchaseOrderProcessor processor)
    {
        _dbContext = dbContext;
        _processor = processor;
    }

    public async Task<PurchaseOrderDto?> PlaceOrderAsync(
        CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(customer => customer.Id == request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return null;
        }

        var distinctProductIds = request.ProductIds.Distinct().ToList();
        var products = await _dbContext.Products
            .Where(product => distinctProductIds.Contains(product.Id))
            .ToListAsync(cancellationToken);
        if (products.Count != distinctProductIds.Count)
        {
            return null;
        }

        var lines = request.ProductIds
            .Select(productId => products.First(product => product.Id == productId))
            .Select(product => new PurchaseOrderLine
            {
                ProductId = product.Id,
                Product = product,
                ProductName = product.Name,
                Price = product.Price
            })
            .ToList();

        var order = new PurchaseOrder
        {
            CustomerId = customer.Id,
            Status = PurchaseOrderStatus.Pending,
            Lines = lines,
            Total = lines.Sum(line => line.Price)
        };

        await _dbContext.PurchaseOrders.AddAsync(order, cancellationToken);

        var context = new PurchaseOrderProcessingContext
        {
            Order = order,
            Customer = customer
        };
        await _processor.ProcessAsync(context, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(order);
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _dbContext.PurchaseOrders
            .AsNoTracking()
            .Include(order => order.Lines)
                .ThenInclude(line => line.Product)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);

        return order is null ? null : ToDto(order);
    }

    public async Task<ShippingSlipDto?> GetShippingSlipAsync(int purchaseOrderId, CancellationToken cancellationToken = default)
    {
        var slip = await _dbContext.ShippingSlips
            .AsNoTracking()
            .Include(slip => slip.Lines)
            .FirstOrDefaultAsync(slip => slip.PurchaseOrderId == purchaseOrderId, cancellationToken);

        return slip is null ? null : ToDto(slip);
    }

    private static PurchaseOrderDto ToDto(PurchaseOrder order)
    {
        return new PurchaseOrderDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Total = order.Total,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            ProcessedAt = order.ProcessedAt,
            Lines = order.Lines
                .Select(line => new PurchaseOrderLineDto
                {
                    ProductId = line.ProductId,
                    ProductName = line.ProductName,
                    ProductType = line.Product.GetType().Name,
                    Price = line.Price
                })
                .ToList()
        };
    }

    private static ShippingSlipDto ToDto(ShippingSlip slip)
    {
        return new ShippingSlipDto
        {
            Id = slip.Id,
            PurchaseOrderId = slip.PurchaseOrderId,
            CustomerId = slip.CustomerId,
            CreatedAt = slip.CreatedAt,
            Lines = slip.Lines
                .Select(line => new ShippingSlipLineDto
                {
                    ProductId = line.ProductId,
                    ProductName = line.ProductName
                })
                .ToList()
        };
    }
}
